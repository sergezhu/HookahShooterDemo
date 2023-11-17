namespace _Game._Scripts.Character.Hero
{
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.Weapons;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IHeroFacade : IDamageableTarget
	{
		Hero Hero { get; }
		Transform CameraTarget { get; }
		public bool IsInputMoving { get; }
		public bool IsDead { get; }
		float FieldOfViewDistance { get; }
		bool IsAttackRollbackDoing { get; }


		public ITarget CurrentTarget { get; }


		ReactiveProperty<bool> IsMeleeAttackActive { get; }
		ReactiveProperty<bool> IsRangeAttackActive { get; }
		IReadOnlyReactiveProperty<Weapon> ActiveWeapon { get; }
		IReadOnlyReactiveProperty<float> RollbackProgressChanged { get; }
		ReactiveCommand Dead { get; }
		int EnemiesCountHavingHeroAsTarget { get; }
		IHeroLiveState LiveState { get; }
		UnitEquip Equip { get; }


		void TryInputMove( bool needRotate );
		void StopMove();
		void RotateToPosition( Vector3 pos, bool force = false );
		void TryRunMeleeAttack();
		void TryRangeAttack();
		void StopAttack();
		void SetDamageableTarget( IDamageableTarget target );
		void ClearDamageableTarget();
		void Move( Vector3 direction, bool needRotate );
		void SetEnemiesCountHavingHeroAsTarget( int count );
		void LockMoving();
		void UnlockMoving();
	}
	
	public class HeroFacade : MonoBehaviour, IHeroFacade
	{
		private Hero _hero;
		private string _name;

		public Transform CameraTarget => transform;
		public Hero Hero => _hero;
		public Vector3 Position => _hero.Position;
		public Quaternion Rotation => transform.rotation;
		public Vector3 AimTargetPosition => _hero.AimTargetPosition;
		public Vector3 Forward => _hero.Forward;
		public string Name => _name;
		public int SiblingIndex => transform.GetSiblingIndex();
		public bool IsEnabled => gameObject.activeSelf;
		public bool IsInputMoving => _hero.IsInputMoving;
		public bool IsDead => _hero.IsDead;

		public ITarget CurrentTarget => _hero.CurrentTarget;
		public IHeroLiveState LiveState => _hero.LiveState;
		public UnitEquip Equip => _hero.Equip;
		public float FieldOfViewDistance => _hero.FieldOfViewDistance;
		

		
		public ReactiveProperty<bool> IsMeleeAttackActive => _hero.IsMeleeAttackActive;
		public ReactiveProperty<bool> IsRangeAttackActive => _hero.IsRangeAttackActive;
		public IReadOnlyReactiveProperty<Weapon> ActiveWeapon => _hero.ActiveWeapon;
		public bool IsAttackRollbackDoing => _hero.IsAttackRollbackDoing;
		public int EnemiesCountHavingHeroAsTarget => _hero.EnemiesCountHavingHeroAsTarget;
		
		public ReactiveCommand<ProjectileRequestData> ProjectileRequested => _hero.ProjectileRequested;
		public IReadOnlyReactiveProperty<float> RollbackProgressChanged => _hero.RollbackProgressChanged;
		public ReactiveCommand Dead => _hero.Dead;


		public Vector3 GetClosestPoint( Vector3 fromPos )
		{
			return Position;
		}

		[Inject]
		private void Construct( Hero hero )
		{
			Debug.Log( "Hero Construct" );
			_hero = hero;
			_name = name;
		}

		public void TryInputMove( bool needRotate ) => _hero.TryInputMove( needRotate );
		public void StopMove() => _hero.StopMove();

		public void Move( Vector3 direction, bool needRotate ) => _hero.Move( direction, needRotate );
		public void RotateToPosition( Vector3 pos, bool force = false ) => _hero.RotateToPosition( pos, force );

		public void TryRunMeleeAttack()
		{
			Debug.Log( $"Hero MeleeAttack!" );
			_hero.TryRunMeleeAttack();
		}

		public void TryRangeAttack()
		{
			Debug.Log( $"Hero RangeAttack!" );
			_hero.TryRunRangeAttack();
		}

		public void StopAttack()
		{
			Debug.Log( $"Hero StopAttack!" );
			_hero.StopAttack();
		}

		public void LockMoving() => _hero.LockMoving();
		public void UnlockMoving() => _hero.UnlockMoving();


		public void SetDamageableTarget( IDamageableTarget target ) => _hero.SetDamageableTarget( target );
		public void ClearDamageableTarget() => _hero.ClearDamageableTarget();
		public void SetEnemiesCountHavingHeroAsTarget( int count ) => _hero.SetEnemiesCountHavingHeroAsTarget( count );

		public void TakeDamage( Damage damage ) => _hero.TakeDamage( damage );
	}
}