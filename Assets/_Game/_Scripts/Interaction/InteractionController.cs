namespace _Game._Scripts.Interaction
{
	using System.Threading;
	using _Game._Scripts.Character.Enemy;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Configs;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Level;
	using _Game._Scripts.Managers.LevelsManagement;
	using _Game._Scripts.UI;
	using _Game._Scripts.UI.Hud;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class InteractionController : IInitializable, ITickable
	{
		private readonly LevelTargetsProvider _targetsProvider;
		private readonly LevelsController _levelsController;
		private readonly HudController _hudController;
		private readonly UIController _uiController;
		private readonly InteractionConfig _interactionConfig;
		private readonly HeroConfig _heroConfig;
		private readonly CompositeDisposable _disposable;

		private bool _isStarted;
		

		private UniTask _interactionTask;
		private CancellationTokenSource _interactionCancelSource;
		private Transform _hintTarget;

		public IHeroFacade Hero => _targetsProvider.MainHeroFacade;

		private float BubbleFlagTimer { get; set; }
		private bool BubbleFlagReady => BubbleFlagTimer <= float.Epsilon;
		private bool CanShowTextBubbleFlag { get; set; }
		private bool CanDisablePreviousHint{ get; set; }
		private bool CanDisablePreviousInteraction{ get; set; }
		private bool IsBattleEnabled { get; set; }
		private ITarget PreviousTarget { get; set; }
		private ITarget CurrentTarget { get; set; }
		private IDamageable NearestDamageableInRange { get; set; }
		

		public InteractionController( LevelTargetsProvider targetsProvider, LevelsController levelsController, HudController hudController, 
									  UIController uiController, InteractionConfig interactionConfig, HeroConfig heroConfig )
		{
			_targetsProvider = targetsProvider;
			_levelsController = levelsController;
			_hudController = hudController;
			_uiController = uiController;
			_interactionConfig = interactionConfig;
			_heroConfig = heroConfig;

			_disposable = new CompositeDisposable();
		}

		public void Initialize()
		{
			_isStarted = false;
			CanShowTextBubbleFlag = true;

			_targetsProvider.NearestAnyTargetInRange
					.Where( _ => _levelsController.LifeCycleState.Value == LevelsController.LifeState.Loaded && IsBattleEnabled == false && Hero.IsDead == false )
					.Subscribe( target => OnNearestTargetChanged( target ) )
					.AddTo( _disposable );

			_targetsProvider.NearestLiveEnemyInAttackRange
				.Where( _ => _levelsController.LifeCycleState.Value == LevelsController.LifeState.Loaded && Hero.IsDead == false )
				.Subscribe( target => OnNearestEnemyInRangeTargetChanged( target ) )
				.AddTo( _disposable );

			_targetsProvider.NearestAggressiveEnemy
				.Where( _ => _levelsController.LifeCycleState.Value == LevelsController.LifeState.Loaded && Hero.IsDead == false )
				.Subscribe( agro => OnNearestAgroEnemyChanged( agro ) )
				.AddTo( _disposable );

			_levelsController.LifeCycleState
				.Where( state => state == LevelsController.LifeState.Loaded )
				.Subscribe( _ => PostInitialize() )
				.AddTo( _disposable );

			_levelsController.LifeCycleState
				.Where( state => state == LevelsController.LifeState.Unloading )
				.Subscribe( _ => CleanUp() )
				.AddTo( _disposable );
		}

		private void PostInitialize()
		{
			Debug.Log( $"LevelUIController PostInitialize" );

			_isStarted = true;
			
			SubscribeOnHero();
		}

		void ITickable.Tick()
		{
		}


		private void SubscribeOnHero()
		{
			var hero = _targetsProvider.MainHeroFacade;
			
			hero.ActiveWeapon
				.Subscribe( weapon => _hudController.OnActiveWeaponChanged( weapon ) )
				.AddTo( _disposable );

			hero.Dead
				.Subscribe( _ => OnHeroDead() )
				.AddTo( _disposable );

			hero.RollbackProgressChanged
				.Subscribe( value => _hudController.SetAttackProgress( value ) )
				.AddTo( _disposable );
		}

		public void TryCancelInteractionForPrevious()
		{
			_interactionCancelSource?.Cancel();
		}

		public void SetHintRoot( Transform target ) => _hintTarget = target;
		

		public void HandleHeroDead()
		{
			IsBattleEnabled = false;
			UnselectInteractables();
		}

		public void UnselectInteractables()
		{
			TryCancelInteractionForPrevious();
			RequestToChangeCurrentTarget( null );
		}

		private ITarget GetTargetForInteraction( out bool hasAggressiveEnemiesOutAttackRange )
		{
			TryCancelInteractionForPrevious();

			var nearestLiveEnemyInRange = _targetsProvider.NearestLiveEnemyInAttackRange.Value;
			var nearestAggressiveEnemy = _targetsProvider.NearestAggressiveEnemy.Value;

			ITarget targetToInteraction = null;

			if ( nearestAggressiveEnemy == null )
			{
				if ( nearestLiveEnemyInRange == null )
				{
					targetToInteraction = _targetsProvider.NearestAnyTargetInRange.Value;
				}
				else
				{
					targetToInteraction = nearestLiveEnemyInRange;
				}
			}
			else
			{
				targetToInteraction = nearestAggressiveEnemy;
			}

			hasAggressiveEnemiesOutAttackRange = nearestLiveEnemyInRange == null && nearestAggressiveEnemy != null;

			return targetToInteraction;
		}

		private void OnNearestTargetChanged( ITarget target )
		{
			if ( Hero.IsDead )
				return;

			RequestToChangeCurrentTarget( target );
		}
		
		private void OnNearestEnemyInRangeTargetChanged( IEnemyFacade target )
		{
			if ( Hero.IsDead )
				return;

			var enemyToAttack = GetTargetForInteraction( out bool hasAggressiveEnemiesOutAttackRange );
			
			if(enemyToAttack != CurrentTarget)
				RequestToChangeCurrentTarget( enemyToAttack );

			/*if ( hasAggressiveEnemiesOutAttackRange == false || _heroConfig.IsAutoRunToAggressiveEnemyEnabled )
			{
				_hero.SetDamageableTarget( enemyToAttack as IDamageableTarget );
			}*/
		}
		
		private void OnNearestAgroEnemyChanged( IEnemyFacade nearestAggroEnemy )
		{
			Hero.SetEnemiesCountHavingHeroAsTarget( _targetsProvider.AggressiveEnemiesCount.Value );

			IsBattleEnabled = nearestAggroEnemy != null;
			
			var enemyToAttack = GetTargetForInteraction(out bool hasAggressiveEnemiesOutAttackRange );
			RequestToChangeCurrentTarget( enemyToAttack );

			/*if ( hasAggressiveEnemiesOutAttackRange == false || _heroConfig.IsAutoRunToAggressiveEnemyEnabled )
			{
				//var nearAgroEnemy = _targetsProvider.EnemyAttackedHeroTargets.ToArray()[0];
				_hero.SetDamageableTarget( enemyToAttack as IDamageableTarget );
			}*/
		}


		private void RequestToChangeCurrentTarget( ITarget target )
		{
			var requestedTargetName = target == null ? "" : target.Name;
			
			if ( CurrentTarget == target )
			{
				Debug.Log( $"Same target : interaction ignored for [{requestedTargetName}]" );
				return;
			}

			if ( IsBattleEnabled && target != null && target is IEnemyFacade == false )
			{
				Debug.Log( $"Battle is active : interaction ignored for [{requestedTargetName}]" );
				return;
			}

			CanShowTextBubbleFlag = true;
			CanDisablePreviousHint = true;
			CanDisablePreviousInteraction = true;

			if ( target == null )
			{
				if ( _targetsProvider.NearestLiveEnemyInAttackRange.Value != null )
				{
					CurrentTarget = _targetsProvider.NearestLiveEnemyInAttackRange.Value;
				}
				else
				{
					CurrentTarget = null;
				}
			}
			else
			{
				CurrentTarget = target;
			}

			UpdateHints();
			UpdateInteractions();

			PreviousTarget = CurrentTarget;
		}

		private void UpdateHints()
		{
			if ( PreviousTarget != null )
				HideInteractionHints( PreviousTarget );

			if ( CurrentTarget != null && CurrentTarget != PreviousTarget )
				ShowInteractionHighlightHint( CurrentTarget );
		}

		private void UpdateInteractions()
		{
			TryCancelInteractionForPrevious();

			if ( CurrentTarget != NearestDamageableInRange )
			{
				NearestDamageableInRange = null;
				Hero.ClearDamageableTarget();
			}

			if ( CurrentTarget != null )
			{
				TryBeginInteraction();
			}
		}

		private void TryBeginInteraction()
		{
			if ( CurrentTarget is IEnemyFacade enemy )
				TryBeginInteractionWithEnemy( enemy );
		}

		private void TryBeginInteractionWithEnemy( IEnemyFacade enemy )
		{
			NearestDamageableInRange = enemy;
			Hero.SetDamageableTarget( enemy );
		}

		private void HideInteractionHints( ITarget target )
		{
			HideInteractionHighlightHint( target );
		}
		
		private void ShowInteractionHighlightHint( ITarget target )
		{
			if(target is IInteractionHintOwner hintOwner)
				hintOwner.ShowInteractionHint();
		}

		private static void HideInteractionHighlightHint( ITarget target )
		{
			if ( target is IInteractionHintOwner hintOwner )
				hintOwner.HideInteractionHint();
		}


		private void CleanUp()
		{
			_interactionCancelSource?.Cancel();
			_interactionCancelSource?.Dispose();
			
			_disposable?.Dispose();
		}

		private void OnHeroDead()
		{
			_uiController.ShowDeadScreen( _interactionConfig.DeadScreenDelay );
		}
	}
}