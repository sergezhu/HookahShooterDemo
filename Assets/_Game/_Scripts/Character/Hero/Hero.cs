namespace _Game._Scripts.Character.Hero
{
	using _Game._Scripts.Character.Animations;
	using _Game._Scripts.Character.Enemy;
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Character.General.Ragdoll;
	using _Game._Scripts.Character.Unit;
	using _Game._Scripts.Configs;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.Level;
	using _Game._Scripts.Managers.Input;
	using _Game._Scripts.Weapons;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class Hero : BaseUnit, ITickable, IInitializable
	{
		private readonly IHeroView _view;
		private readonly HeroAIController _heroAIController;
		private readonly HeroInput _input;
		private readonly HeroConfig _heroConfig;
		private readonly HeroLiveState _heroLiveState;
		private readonly UnitAnimatorController _unitAnimatorController;
		private readonly UnitAttackSystem _attackSystem;
		private readonly RagdollController _ragdollController;
		private readonly UnitEquip _equip;

		private bool _isInputMoving;
		private Vector3 _moveDirection;

		private int _projectileRequestID;
		private int _tryRangeRequestID;

		public Hero(IHeroView view, HeroAIController heroAIController, HeroInput input, HeroConfig heroConfig, HeroLiveState heroLiveState, 
					UnitAnimatorController unitAnimatorController, UnitEquip equip, RagdollController ragdollController, 
					UnitCurrentTargetProvider unitCurrentTargetProvider, UnitAttackSystem attackSystem) : base( view, unitCurrentTargetProvider, attackSystem)
		{
			_view = view;
			_heroAIController = heroAIController;
			_input = input;
			_heroConfig = heroConfig;
			_heroLiveState = heroLiveState;
			_unitAnimatorController = unitAnimatorController;
			_equip = equip;
			_ragdollController = ragdollController;
			_attackSystem = attackSystem;

			Debug.Log( $"Hero : _heroEquip is null {_equip == null}" );
		}


		public Transform CameraTarget => _view.CameraTarget;
		public IHeroLiveState LiveState => _heroLiveState;
		public UnitEquip Equip => _equip;

		public bool IsDead => _heroLiveState.IsDead;
		public bool IsInputMoving => _isInputMoving;
		public bool IsAttackRollbackDoing => _attackSystem.IsAttackRollbackDoing;
		public float FieldOfViewDistance => _heroConfig.FieldOfViewDistance;


		public ReactiveProperty<bool> IsMeleeAttackActive { get; } = new ReactiveProperty<bool>();
		public ReactiveProperty<bool> IsRangeAttackActive { get; } = new ReactiveProperty<bool>();
		public IReadOnlyReactiveProperty<Weapon> ActiveWeapon { get; private set; }
		public IReadOnlyReactiveProperty<float> RollbackProgressChanged { get; private set; }

		private bool IsInputRangeAttackActive { get; set; }
		private bool IsInputMeleeAttackActive { get; set; }
		public int EnemiesCountHavingHeroAsTarget { get; private set; }


		public new void Initialize()
		{
			base.Initialize();
			
			_view.SetHealth( _heroConfig.MaxHealth );
			ActiveWeapon = _equip.CurrentWeapon.ToReadOnlyReactiveProperty();
			RollbackProgressChanged = _attackSystem.RollbackProgressChanged.ToReadOnlyReactiveProperty();

			PostprocessOfUpdateEquip();
		}
		
		public void Tick()
		{
			_isInputMoving = _input.IsMoving;
			
			//Debug.Log( $"is input moving : {_isMoving}" );
			_moveDirection = _input.WorldDirection;

			_heroAIController.Tick();

			//CheckOfChangeTarget();
			CheckIfAttacksActive();
			CheckOfEnemyInRange();
		}

		public void Activate()
		{
			_heroAIController.Activate();
			_attackSystem.Activate();

			PostprocessOfUpdateEquip();
			Subscribe();
		}

		public void Deactivate()
		{
			_heroAIController.Deactivate();
			_attackSystem.Deactivate();
			_unitAnimatorController.CleanUp();

			Unsubscribe();
		}

		private void Subscribe()
		{
			_equip.CurrentWeapon
				.Subscribe( _ =>
				{
					PostprocessOfUpdateEquip();
				} )
				.AddTo( _disposable );

			_attackSystem.RollbackProgressChanged
				.Subscribe( value => _input.SetWeaponRollbackProgress( value ) )
				.AddTo( _disposable );

			_input.IsAttackButtonPressed
				.Subscribe( isPressed => OnAttackButtonPressed( isPressed ) )
				.AddTo( _disposable );

			_heroLiveState.Health
				.Subscribe( hp => _view.SetHealth( hp ) )
				.AddTo( _disposable );
		}

		private void Unsubscribe()
		{
			_disposable.Clear();
			_heroLiveState.CleanUp();
		}

		public void AttachTargetsProvider( LevelTargetsProvider targetsProvider ) => _heroAIController.AttachTargetsProvider( targetsProvider );

		public void DetachTargetsProvider() => _heroAIController.DetachTargetsProvider();


		public void TryInputMove(bool needRotate)
		{
			if ( _isInputMoving )
			{
				_heroAIController.TryNavmeshMoveAlongDir( _moveDirection, needRotate );
			}
		}

		public void Move( Vector3 dir, bool needRotate )
		{
			Debug.Log( $"Move dir : {dir}" );
			_heroAIController.TryNavmeshMoveAlongDir( dir, needRotate );
		}

		public void StopMove()
		{
			_heroAIController.StopMove();
		}

		public void RotateToPosition( Vector3 pos, bool force = false )
		{
			_heroAIController.RotateToPosition( pos, force );
		}

		private void OnAttackButtonPressed( bool isPressed )
		{
			Debug.Log( $"Attack button pressed : {isPressed}" );
			UpdateAttacksInput( isPressed );
		}

		private void UpdateAttacksInput( bool isPressed )
		{
			if ( _equip.IsActiveWeaponRange )
			{
				IsInputRangeAttackActive = isPressed;
				IsInputMeleeAttackActive = false;
			}

			if ( _equip.IsActiveWeaponMelee )
			{
				if ( isPressed )
					IsInputMeleeAttackActive = !IsInputMeleeAttackActive;

				IsInputRangeAttackActive = false;
			}
		}

		public void TakeDamage( Damage damage )
		{
			if ( _heroLiveState.IsDead )
			{
				Debug.LogWarning( $"You try damage upon hero, but hero is dead" );
				return;
			}

			Debug.Log( $"Hero : incoming damage : {damage.Value}" );
			_heroLiveState.ChangeHealth( -1f * damage.Value );

			if ( _heroLiveState.IsDead )
			{
				HandleDead( damage );
			}
		}

		private void HandleDead( Damage damage )
		{
			_unitAnimatorController.HandleDead();
			_heroAIController.Deactivate();
			_ragdollController.EnableRagdoll();
			_ragdollController.SetDeadLayer();
			_ragdollController.Push( damage );

			Dead.Execute();
			
			Debug.Log( "Hero is dead!" );
		}

		public void StopAttack()
		{
			_attackSystem.StopAttack();
		}

		public void LockMoving()
		{
			_input.LockMoveInput = true;
			//_heroAIController.Lock();
		}

		public void UnlockMoving()
		{
			_input.LockMoveInput = false;
			//_heroAIController.Unlock();
		}

		public bool TryRunMeleeAttack() => _attackSystem.TryStartMeleeAttack();
		public bool TryRunRangeAttack() => _attackSystem.TryStartRangeAttack();


		private void PostprocessOfUpdateEquip()
		{
			CheckIfAttacksActive();
			
			_view.UpdateWeapon( _equip.CurrentWeapon.Value );
		}

		private void CheckIfAttacksActive()
		{
			if ( _equip.IsActiveWeaponMelee )
				IsInputRangeAttackActive = false;
			
			if( _equip.IsActiveWeaponRange )
				IsInputMeleeAttackActive = false;
			
			if ( _isInputMoving )
			{
				IsInputMeleeAttackActive = false;
				IsMeleeAttackActive.Value = false;

				if ( _heroConfig.IsShootingWhenMovingEnabled )
				{
					IsRangeAttackActive.Value = _equip.IsActiveWeaponRange && (_heroConfig.IsAutoRangeAttackEnabled || IsInputRangeAttackActive);
				}
				else
				{
					IsRangeAttackActive.Value = false;
				}
			}
			else
			{
				IsMeleeAttackActive.Value = _equip.IsActiveWeaponMelee && (_heroConfig.IsAutoMeleeAttackEnabled || IsInputMeleeAttackActive);
				IsRangeAttackActive.Value = _equip.IsActiveWeaponRange && (_heroConfig.IsAutoRangeAttackEnabled || IsInputRangeAttackActive);
			}
		}

		private void CheckOfEnemyInRange()
		{
			if ( CurrentTarget == null || CurrentTarget is IEnemyFacade == false || _equip.HasWeapon == false)
			{
				_input.SetIsEnemyInAttackRange( false );
				return;
			}

			var sqrDist = (CurrentTarget.Position - Position).sqrMagnitude;
			var sqrAttackDist = _equip.WeaponAttackDistance * _equip.WeaponAttackDistance;
			var inRange = sqrDist <= sqrAttackDist;

			_input.SetIsEnemyInAttackRange( inRange );

			var aimDist = _equip.WeaponAttackDistance + _heroConfig.DeltaAimDistance;
			var sqrAimDist = aimDist * aimDist;
			var inAimRange = sqrDist <= sqrAimDist;

			if ( inAimRange && _equip.IsActiveWeaponRange )
			{
				// ebable aim
			}
			else
			{
				// disable aim
			}
		}

		public void SetDamageableTarget( IDamageableTarget damageableTarget )
		{
			string targetName = damageableTarget == null ? "" : damageableTarget.Name;
			Debug.Log( $"[Hero] SetDamageableTarget : [{targetName}]" );

			if ( IsDamageDealingPaused == false )
			{
				CurrentTarget = damageableTarget;
			}
			else
			{
				CurrentTarget = null;
				CachedTarget = damageableTarget;
			}

			_heroAIController.SetDamageableTarget( (IDamageableTarget)CurrentTarget );
		}

		public void ClearDamageableTarget()
		{
			CurrentTarget = null;
			CachedTarget = null;

			_heroAIController.SetDamageableTarget( null );
		}

		public void SetEnemiesCountHavingHeroAsTarget( int enemiesCountHavingHeroAsTarget )
		{
			EnemiesCountHavingHeroAsTarget = enemiesCountHavingHeroAsTarget;
		}

		public void InitializeRagdoll( IDamageableTarget facade )
		{
			_ragdollController.Construct( facade, Layers.Hero, Layers.DeadHero );
			_ragdollController.DisableRagdoll();
			_ragdollController.SetNormalLayer();
		}
	}
}