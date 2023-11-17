namespace _Game._Scripts.Character.Enemy
{
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Character.Unit;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Weapons;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class Enemy : BaseUnit, ITickable, IInitializable
	{
		private readonly EnemyView _view;
		private readonly EnemyConfig _enemyConfig;
		private readonly UnitEquip _unitEquip;
		private readonly EnemyLiveState _enemyLiveState;
		private readonly UnitAttackSystem _attackSystem;
		private readonly ITarget[] _levelTargets;
		private readonly EnemyAIController _enemyAIController;
		private Coroutine _attackRoutine;


		public Enemy( EnemyView view, EnemyAIController enemyAIController, EnemyConfig enemyConfig, UnitEquip unitEquip, EnemyLiveState enemyLiveState, 
					  UnitAttackSystem attackSystem, UnitCurrentTargetProvider currentTargetProvider) 
			: base(view, currentTargetProvider, attackSystem)
		{
			_view = view;
			_enemyAIController = enemyAIController;
			_enemyConfig = enemyConfig;
			_unitEquip = unitEquip;
			_enemyLiveState = enemyLiveState;
			_attackSystem = attackSystem;

			RemoveSelfAsTargetRequest = new ReactiveCommand();
		}
		
		public float Radius => _view.Radius;
		
		public float FieldOfViewDistance => _enemyConfig.FieldOfViewDistance;

		public bool IsAttackRollbackDoing => _attackSystem.IsAttackRollbackDoing;
		public UnitEquip Equip => _unitEquip;


		public bool IsAggro => _enemyAIController.IsAggro;
		public bool IsDead => _enemyLiveState.IsDead;

		public ReactiveCommand RemoveSelfAsTargetRequest { get; }
		public ReactiveProperty<bool> IsShown { get; } = new ReactiveProperty<bool>();


		public new void Initialize()
		{
			base.Initialize();
			
			IsShown.Value = _view.IsShown;
		}

		
		public void Tick()
		{
			IsShown.Value = _view.IsShown;

			_enemyAIController?.Tick();

			if ( _enemyAIController?.CurrentTarget is IDamageableTarget damageableTarget )
			{
				SetTarget( damageableTarget );
			}
			else if ( _enemyAIController?.CurrentTarget is IMovingTarget movingTarget )
			{
				SetTarget( movingTarget );
			}
			else
			{
				RemoveTarget();
			}
		}

		public void Activate()
		{
			Debug.Log( $"[Enemy] {Name} : activated" );
			
			_enemyAIController.Activate();
			
			Subscribe();
		}

		public void Deactivate()
		{
			Debug.Log( $"[Enemy] {Name} : deactivated" );
			
			_enemyAIController.Deactivate();
			
			Unsubscribe();
		}

		private void Subscribe()
		{
		}

		private void Unsubscribe()
		{
			_disposable.Clear();
		}

		public void CleanUp()
		{
			Unsubscribe();
			
			_enemyLiveState.CleanUp();
			_enemyAIController.CleanUp();
			_unitEquip.CleanUp();
			_enemyLiveState.CleanUp();
		}

		public void AttachChild( Transform child )
		{
			_view.AttachChild( child );
		}

		public void DetachSpawnPoint()
		{
		}

		public void TryFollowPath()
		{
			_enemyAIController.TryFollowPath();
		}

		public void TryPavePath(Vector3 pos)
		{
			_enemyAIController.TryPavePath( pos );
		}

		public void StopMove()
		{
			_enemyAIController.StopMove();
		}

		public void RotateToPosition( Vector3 pos )
		{
			_enemyAIController.RotateToPosition( pos );
		}


		public void LockMoving()
		{
			_enemyAIController.Lock();
		}

		public void UnlockMoving()
		{
			_enemyAIController.Unlock();
		}

		private void SetTarget( ITarget target )
		{
			if ( IsDamageDealingPaused == false )
			{
				CurrentTarget = target;
			}
			else
			{
				CurrentTarget = null;
				CachedTarget = target;
			}
		}

		private void RemoveTarget()
		{
			CurrentTarget = null;
			CachedTarget = null;
		}

		public void TakeDamage( Damage damage )
		{
			if ( _enemyLiveState.IsDead )
			{
				Debug.LogWarning( $"You try damage upon enemy {Name}, but enemy is dead" );
				return;
			}
			
			Debug.Log( $"{Name} take damage : {damage.Value}" );

			_enemyLiveState.Health.Value = Mathf.Max( _enemyLiveState.Health.Value - damage.Value, 0 );
			
			_enemyAIController.EnableAggro();
			_view.SetHealth( _enemyLiveState.Health.Value );

			if ( _enemyLiveState.IsDead )
			{
				HandleDead( damage );
			}
		}

		private void HandleDead( Damage damage )
		{
			_enemyAIController.Deactivate();

			Dead.Execute();
			DestroyWithAnimate();

			Debug.Log( $"Enemy {Name} is dead!" );
		}

		public void DestroyWithAnimate()
		{
			Deactivate();
			
			RemoveSelfAsTargetRequest.Execute();

			_view.DestroyWithAnimate( null );
		}

		public void TryMeleeAttack()
		{
			_attackSystem.TryStartMeleeAttack();
		}

		public void TryRangeAttack()
		{
			_attackSystem.TryStartRangeAttack();
		}

		public void StopAttack() => _attackSystem.StopAttack();
		public void ShowInteractionHint() => _view.ShowInteractionHint();
		public void HideInteractionHint() => _view.HideInteractionHint();

		/*private IEnumerator MeleeAttackRoutine()
		{
			var waiter = new WaitForSeconds( 0.1f );

			while ( true )
			{
				_attackSystem.TryStartMeleeAttack();
				yield return waiter;
			}
		}

		private IEnumerator RangeAttackRoutine()
		{
			var waiter = new WaitForSeconds( 0.1f );

			while ( true )
			{
				_attackSystem.TryStartRangeAttack();
				yield return waiter;
			}
		}*/
	}
}