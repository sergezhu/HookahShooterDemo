namespace _Game._Scripts.Weapons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using _Game._Scripts.Character.General.Ragdoll;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Configs;
	using _Game._Scripts.Enums;
	using _Game._Scripts.FX;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Managers.LevelsManagement;
	using _Game._Scripts.Utilities.Extensions;
	using Cysharp.Threading.Tasks;
	using QFSW.MOP2;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IProjectileController
	{
		void HandleProjectileRequest( ProjectileRequestData data );
		void PauseProjectiles();
		void ResumeProjectiles();
	}
	
	public class ProjectileController : IInitializable, IProjectileController
	{
		private readonly MasterObjectPooler _pooler;
		private readonly ProjectilePools _pools;
		private readonly BattleConfig _battleConfig;
		private readonly LevelsController _levelsController;
		private IHeroFacade _hero;
		private bool _isStarted;
		private List<Projectile> _activeProjectiles;
		
		private readonly CompositeDisposable _disposable;
		private readonly CompositeDisposable _projectilesDisposable;
		private readonly RaycastHit[] _areaHits;
		private readonly List<IDamageable> _areaDamageableTargets;
		private CancellationTokenSource _destroyCancellationSource;

		public ProjectileController(MasterObjectPooler pooler, ProjectilePools pools, BattleConfig battleConfig, LevelsController levelsController )
		{
			_pooler = pooler;
			_pools = pools;
			_battleConfig = battleConfig;
			_levelsController = levelsController;
			_activeProjectiles = new List<Projectile>();
			_areaHits = new RaycastHit[20];
			_areaDamageableTargets = new List<IDamageable>( 20 );

			_disposable = new CompositeDisposable();
			_projectilesDisposable = new CompositeDisposable();
		}

		public void Initialize()
		{
			_levelsController.LifeCycleState
				.Where( s => s == LevelsController.LifeState.Loaded )
				.Subscribe( _ => PostInitialize() )
				.AddTo( _disposable );

			_levelsController.LifeCycleState
				.Where( s => s == LevelsController.LifeState.Unloading )
				.Subscribe( _ => CleanUp() )
				.AddTo( _disposable );

			_destroyCancellationSource = new CancellationTokenSource();
		}

		private void PostInitialize()
		{
			_isStarted = false;

			foreach ( var pool in _pools.GetUniqueFlyPools() )
			{
				if(_pooler.HasPool( pool.PoolName ))
					continue;
				
				_pooler.AddPool( pool );
				_pooler.Populate( pool.PoolName, 20 );
			}

			foreach ( var pool in _pools.GetUniqueHitFxPools() )
			{
				if ( _pooler.HasPool( pool.PoolName ) )
					continue;
				
				_pooler.AddPool( pool );
				_pooler.Populate( pool.PoolName, 20 );
			}

			foreach ( var pool in _pools.GetUniqueAreaHitFxPools() )
			{
				if ( _pooler.HasPool( pool.PoolName ) )
					continue;
				
				_pooler.AddPool( pool );
				_pooler.Populate( pool.PoolName, 20 );
			}

			foreach ( var pool in _pools.GetUniqueHitDecalPools() )
			{
				if ( _pooler.HasPool( pool.PoolName ) )
					continue;
				
				_pooler.AddPool( pool );
				_pooler.Populate( pool.PoolName, 20 );
			}
		}

		public void HandleProjectileRequest( ProjectileRequestData data )
		{
			int separatedProjectileCount = Mathf.Max(1, data.DatabaseWeapon.SeparateDamageCount);
			
			Debug.Log( $"Projectile requested for {data.DatabaseWeapon.Name}, {data.ID}, projectilesCount : {separatedProjectileCount}" );

			for ( int projIndex = 0; projIndex < separatedProjectileCount; projIndex++ )
				LaunchProjectile( data, projIndex );
			
		}

		public void PauseProjectiles()
		{
			_activeProjectiles.ForEach( proj => proj.Pause() );
		}

		public void ResumeProjectiles()
		{
			_activeProjectiles.ForEach( proj => proj.Resume() );
		}

		private void LaunchProjectile( ProjectileRequestData data, int projectileIndex )
		{
			var pool = _pools.GetFlyPool( data.DatabaseWeapon.ID );
			var projectile = pool.GetObjectComponent<Projectile>( data.WeaponView.ShootPoint.position );

			projectile.gameObject.SetActive( false );

			var startPoint = data.WeaponView.ShootPoint;
			var mask = -1;

			switch ( data.Owner )
			{
				case ProjectileOwner.None:
					break;
				case ProjectileOwner.Player:
					mask = (1 << Layers.Enemies) | (1 << Layers.Navigation);
					break;
				case ProjectileOwner.Enemy:
					mask = (1 << Layers.Hero) | (1 << Layers.Navigation);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			projectile.Construct( data, mask, _pools, projectileIndex );
			projectile.AlignToParent( startPoint, data.AimPosition );

			projectile.gameObject.SetActive( true );
			projectile.StartFly();

			SetProjectileAsActive( projectile );
		}

		private void CleanUp()
		{
			foreach ( var pool in _pools.GetUniqueFlyPools() )
			{
				_pooler.DestroyPool(pool.PoolName);
			}

			foreach ( var pool in _pools.GetUniqueHitFxPools() )
			{
				_pooler.DestroyPool( pool.PoolName );
			}

			foreach ( var pool in _pools.GetUniqueHitDecalPools() )
			{
				_pooler.DestroyPool( pool.PoolName );
			}

			foreach ( var pool in _pools.GetUniqueAreaHitFxPools() )
			{
				_pooler.DestroyPool( pool.PoolName );
			}
			
			_disposable.Clear();
			_projectilesDisposable.Clear();

			_destroyCancellationSource.Cancel(false);
		}

		private void OnProjectileHit( Projectile projectile, ProjectileHitData data )
		{
			var weaponID = projectile.WeaponID;
			var mask = projectile.Mask;
			//var mask = (1 << Layers.Enemies);

			projectile.EndFly();
			SetProjectileAsInactive( projectile );

			var flyFxPool = _pools.GetFlyPool( weaponID );
			
			ReleaseFlyFxRoutine( projectile, flyFxPool ).Forget();
			TryRunHitFxRoutine( data.HitFxPoolName, data.HitPosition ).Forget();

			var hitDamage = new Damage( EWeaponDistanceType.Range )
			{
				Value = data.Damage, 
				HitForce = data.HitForceModifier * data.HitDirection, 
				Position = data.HitPosition
			};

			if ( data.HitAreaDamageEnabled )
			{
				TryRunHitFxRoutine( data.AreaHitFxPoolName, data.HitPosition ).Forget();
				
				Physics.SphereCastNonAlloc( data.HitPosition, data.HitAreaDamageRadius, Vector3.up, _areaHits, 0, mask );

				/*{
					var uniqueDamageablesNotNull = _areaHits
						.Where( hit => hit.collider != null )
						.Select( hit => GetDamageableOnHit( hit ) )
						.Where( d => d != null )
						.Distinct()
						.ToArray();

					var hitNotNullnames = uniqueDamageablesNotNull
						.Select( d => $"[{((IDamageableTarget)d).Name}] " )
						.ToArray();

					var concatNames = string.Concat( hitNotNullnames );
					Debug.Log( $"<color=blue>Area hit, mask = {mask}, targets count : [{uniqueDamageablesNotNull.Length}], names : {concatNames} </color>" );
				}*/
				
				ApplyAreaDamage( data, ref hitDamage );
			}

			data.DamageableTarget?.TakeDamage( hitDamage );
		}

		private void ApplyAreaDamage( ProjectileHitData data, ref Damage hitDamage )
		{
			_areaDamageableTargets.Clear();

			foreach ( var hit in _areaHits )
			{
				if ( hit.collider == null )
					break;

				var vector = (hit.transform.position - data.HitPosition).WithY( 0 );
				var magnitude = Vector3.Magnitude( vector );
				var dir = vector.normalized;

				var damageable = GetDamageableOnHit( hit );

				var damageableName = damageable == null ? "" : ((ITarget)damageable).Name;

				if ( damageable == null || _areaDamageableTargets.Contains( damageable ) )
					continue;

				_areaDamageableTargets.Add( damageable );

				var areaDamageModifier = _battleConfig.ExplosionDamageFactorFromRadius.Evaluate( magnitude / data.HitAreaDamageRadius );
				var areaForceModifier = _battleConfig.ExplosionForceFactorFromRadius.Evaluate( magnitude / data.HitAreaDamageRadius );
				areaForceModifier *= _battleConfig.BaseExplosionHitRagdollForce;
				var forceModifierY = 0.5f;

				if ( damageable == data.DamageableTarget )
				{
					var totalHitDamage = new Damage()
					{
						Value = hitDamage.Value + data.HitAreaDamageValue * areaDamageModifier,
						HitForce = hitDamage.HitForce + data.HitForceModifier * areaForceModifier * dir + data.HitForceModifier * areaForceModifier *
							forceModifierY * Vector3.up,
						Position = hitDamage.Position
					};

					hitDamage = totalHitDamage;
					//Debug.Log( $"<color=blue>Area hit same, dir : {dir}, damageableName : [{damageableName}] </color>" );
				}
				else
				{
					var areaHitDamage = new Damage()
					{
						Value = data.HitAreaDamageValue * areaDamageModifier,
						HitForce = data.HitForceModifier * areaForceModifier * dir + data.HitForceModifier * areaForceModifier *
							forceModifierY * Vector3.up,
						Position = data.HitPosition
					};

					damageable.TakeDamage( areaHitDamage );

					//Debug.Log( $"<color=blue>Area hit other, dir : {dir}, damageableName : [{damageableName}] </color>" );
				}
			}
		}

		private IDamageable GetDamageableOnHit( RaycastHit hit )
		{
			hit.transform.TryGetComponent<IDamageable>( out var damageable );

			if ( damageable == null )
			{
				hit.transform.TryGetComponent<Bone>( out var bone );

				if ( bone != null )
				{
					damageable = bone.OwnerDamageableTarget;
					
					/*Debug.Log( $"<color=blue>Area hit, bone : {bone.name}, " +
							   $"damageableName : [{bone.OwnerDamageableTarget.Name}], " +
							   $"layer : {((MonoBehaviour)damageable).gameObject.layer} / {bone.gameObject.layer} </color>" );	*/
				}
			}

			return damageable;
		}

		private async UniTaskVoid ReleaseFlyFxRoutine( Projectile projectile, ObjectPool flyFxPool )
		{
			var delay = projectile.FX.TrailLifeTime;

			await UniTask
				.Delay( TimeSpan.FromSeconds( delay ), ignoreTimeScale: false )
				.AttachExternalCancellation( _destroyCancellationSource.Token );

			_pooler.Release( projectile.gameObject, flyFxPool.PoolName );
		}
		
		
		private async UniTaskVoid TryRunHitFxRoutine( string hitFxPoolName, Vector3 hitPosition )
		{
			if ( string.IsNullOrEmpty( hitFxPoolName ) )
				return;

			var hitFx = _pooler.GetObjectComponent<FXWrapper>( hitFxPoolName, hitPosition );
			hitFx.PlayProperly();

			await UniTask
				.Delay( TimeSpan.FromSeconds( hitFx.LifeTime * 2f ), ignoreTimeScale: false )
				.AttachExternalCancellation( _destroyCancellationSource.Token );

			_pooler.Release( hitFx.gameObject, hitFxPoolName );
		}

		/*private async UniTaskVoid AreaHitFxRoutine( ProjectileHitData data )
		{
			var hitAreaFxPoolName = data.AreaHitFxPoolName;

			if ( string.IsNullOrEmpty( hitAreaFxPoolName ) )
				return;

			var areaHitFx = _pooler.GetObjectComponent<FXWrapper>( hitAreaFxPoolName, data.HitPosition );
			areaHitFx.Play();

			await UniTask.Delay( TimeSpan.FromSeconds( areaHitFx.LifeTime ), ignoreTimeScale: false );

			_pooler.Release( areaHitFx.gameObject, hitAreaFxPoolName );
		}*/

		private void OnProjectileOutOfRange( Projectile projectile )
		{
			var weaponID = projectile.WeaponID;
			var pool = _pools.GetFlyPool( weaponID );

			projectile.EndFly();
			SetProjectileAsInactive( projectile );

			_pooler.Release( projectile.gameObject, pool.PoolName );
		}

		private void SetProjectileAsActive( Projectile projectile )
		{
			if ( _activeProjectiles.Contains( projectile ) == false )
			{
				_activeProjectiles.Add( projectile );
				ResubscribeProjectiles();
			}
		}

		private void SetProjectileAsInactive( Projectile projectile )
		{
			if ( _activeProjectiles.Contains( projectile ) )
			{
				_activeProjectiles.Remove( projectile );
				ResubscribeProjectiles();
			}
		}

		private void ResubscribeProjectiles()
		{
			_projectilesDisposable.Clear();

			_activeProjectiles.ForEach( proj =>
			{
				proj.Hit
					.Subscribe( data => OnProjectileHit( proj, data ) )
					.AddTo( _projectilesDisposable );

				proj.OutOfRange
					.Subscribe( _ => OnProjectileOutOfRange( proj ) )
					.AddTo( _projectilesDisposable );
			} );
		}
	}
}