namespace _Game._Scripts.Character.Unit
{
	using _Game._Scripts.Character.Animations;
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Configs;
	using _Game._Scripts.Enums;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.Weapons;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UnitAttackSystem : IInitializable
	{
		private readonly UnitEquip _equip;
		private readonly IUnitView _unitView;
		private readonly IAnimatorController _animatorController;
		private readonly BattleConfig _battleConfig;
		private readonly UnitCurrentTargetProvider _unitCurrentTargetProvider;

		private int _projectileRequestID;
		private readonly RollbackWeaponTimer _weaponRollbackTimer;
		private readonly CompositeDisposable _disposable;

		public UnitAttackSystem(UnitEquip equip, IUnitView unitView, [InjectOptional] IAnimatorController animatorController, 
								BattleConfig battleConfig, UnitCurrentTargetProvider unitCurrentTargetProvider)
		{
			_equip = equip;
			_unitView = unitView;
			_animatorController = animatorController;
			_battleConfig = battleConfig;
			_unitCurrentTargetProvider = unitCurrentTargetProvider;

			_disposable = new CompositeDisposable();
			_weaponRollbackTimer = new RollbackWeaponTimer();
		}

		public ReactiveCommand<ProjectileRequestData> ProjectileRequested { get; } = new ReactiveCommand<ProjectileRequestData>();
		public ReactiveCommand<float> RollbackProgressChanged { get; private set; }

		public bool IsAttackRollbackDoing => _weaponRollbackTimer.IsAttackRollbackDoing;


		public void Initialize()
		{
			RollbackProgressChanged = _weaponRollbackTimer.RollbackProgressChanged;
		}

		public void Activate()
		{
			
		}

		public void Deactivate()
		{
			StopAttack();
			_disposable.Clear();
		}


		public void StopAttack()
		{
		}

		public bool TryStartMeleeAttack() 
		{
			if ( _equip.IsActiveWeaponMelee == false || IsAttackRollbackDoing )
				return false;

			var target = _unitCurrentTargetProvider.CurrentTarget;
			var distanceToTarget = Vector3.Distance( _unitView.Position, target.Position );
			var inAttackRange = distanceToTarget <= _equip.WeaponAttackDistance;
			
			if ( inAttackRange )
			{
				_animatorController?.DoMeleeAnimation();
				_weaponRollbackTimer.SetRollback( _equip.CurrentWeapon.Value.AttackDelay );
				
				OnMeleeHit();
			}
			else
			{
				Debug.Log( $"Target is not already in attack range. Attack failed" );
			}

			return inAttackRange;
		}

		public bool TryStartRangeAttack()
		{
			if ( _equip.IsActiveWeaponRange == false || IsAttackRollbackDoing )
				return false;

			var target = (IDamageableTarget)_unitCurrentTargetProvider.CurrentTarget;
			var distanceToTarget = Vector3.Distance( _unitView.Position, target.Position );

			if ( distanceToTarget <= _equip.WeaponAttackDistance )
			{
				OnRangeFire();

				_animatorController?.DoShootAnimation();
				_weaponRollbackTimer.SetRollback( _equip.CurrentWeapon.Value.AttackDelay );

				return true;
			}

			return false;
		}


		private void OnMeleeHit()
		{
			var weapon = _equip.CurrentWeapon.Value;
			var damageable = (IDamageableTarget)_unitCurrentTargetProvider.CurrentTarget;

			if ( weapon == null || damageable == null )
			{
				Debug.LogWarning( "Invalid operation. Hit detected but weapon or target is null" );
				return;
			}

			var target = (ITarget)damageable;
			var distanceToTarget = Vector3.Distance( _unitView.Position, target.Position );
			var damage = weapon.RandomPower;

			if ( distanceToTarget <= _equip.WeaponAttackDistance )
			{
				Debug.Log( $"Attack is successfully! damage {damage}" );
				damageable.TakeDamage( new Damage( EWeaponDistanceType.Melee )
				{
					Value = damage,
					HitForce = _battleConfig.BaseMeleeDeadHitRagdollForce * _equip.CurrentWeapon.Value.HitForceModifier * _unitView.Forward
				} );
			}
			else
			{
				Debug.Log( $"Attack is failed!" );
			}
		}


		private void OnRangeFire()
		{
			TryFireProjectileOrStop();
		}

		private void TryFireProjectileOrStop()
		{
			if ( _unitCurrentTargetProvider.CurrentTarget == null )
			{
				Debug.Log( $"Target is null, Projectile request is ignored!" );
				StopAttack();
			}
			else
			{
				FireProjectile();
			}
		}

		private void FireProjectile()
		{
			_projectileRequestID++;
			//Debug.Log( $"Projectile is fired! ({_projectileRequestID})" );

			var rangeWeapon = _equip.CurrentWeapon.Value as RangeWeapon;

			if ( rangeWeapon == null )
			{
				Debug.LogError( $"Range weapon is not equipped but you tried fire a projectile!" );
				return;
			}
			
			var weaponView = _unitView.GetWeaponViewByName( rangeWeapon.Name );
			var damageable = (IDamageableTarget)_unitCurrentTargetProvider.CurrentTarget;

			ProjectileRequestData projectileRequestData = default;

			projectileRequestData = new ProjectileRequestData()
			{
				ID = _projectileRequestID,
				DatabaseWeapon = rangeWeapon,
				Damage = rangeWeapon.RandomPower,
				WeaponView = weaponView,
				AimPosition = damageable.AimTargetPosition,
				HitForceModifier = _battleConfig.BaseRangeDeadHitRagdollForce,
				Owner = ProjectileOwner.Player
			};

			ProjectileRequested.Execute( projectileRequestData );
		}
	}
}