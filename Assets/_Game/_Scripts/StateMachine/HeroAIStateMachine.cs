namespace _Game._Scripts.StateMachine
{
    using _Game._Scripts.StateMachine.Base;

    public class HeroAIStateMachine : BaseAIStateMachine
    {
        /*private CompositeDisposable _disposable;
    private readonly PlayerIdleState _idleState;
    private readonly PlayerOnlyMoveState _onlyMoveState;
    private readonly PlayerMeleeAttackState _meleeAttackState;
    private readonly PlayerRangeAttackState _rangeAttackState;
    private readonly PlayerOpeningContainerState _openingContainer;
    private readonly PlayerPrepareMiningState _prepareMining;
    private readonly PlayerPrepareAttackState _prepareAttack;
    private readonly PlayerMiningAttackState _miningAttack;
    private readonly PlayerDeadState _deadState;
    private readonly PlayerTutorialStunnedState _tutorialStunnedState;
    private readonly HeroStatistics _stats;


    public HeroAIStateMachine( PlayerIdleState idle, PlayerOnlyMoveState onlyMove, PlayerMeleeAttackState meleeAttack, PlayerRangeAttackState rangeAttack,
                               PlayerOpeningContainerState openingContainer, PlayerPrepareMiningState prepareMining,  PlayerPrepareAttackState prepareAttack, 
                               PlayerMiningAttackState miningAttack, PlayerDeadState deadState, PlayerTutorialStunnedState tutorialStunnedState,
                               HeroStatistics stats) : base()
    {
        _states[typeof(PlayerIdleState)] = idle;
        _states[typeof(PlayerOnlyMoveState)] = onlyMove;
        _states[typeof(PlayerMeleeAttackState)] = meleeAttack;
        _states[typeof(PlayerRangeAttackState)] = rangeAttack;
        _states[typeof(PlayerOpeningContainerState)] = openingContainer;
        _states[typeof(PlayerPrepareAttackState)] = prepareAttack;
        _states[typeof(PlayerPrepareMiningState)] = prepareMining;
        _states[typeof(PlayerMiningAttackState)] = miningAttack;
        _states[typeof(PlayerDeadState)] = deadState;
        _states[typeof(PlayerTutorialStunnedState)] = tutorialStunnedState;

        _idleState = idle;
        _onlyMoveState = onlyMove;
        _meleeAttackState = meleeAttack;
        _rangeAttackState = rangeAttack;
        _openingContainer = openingContainer;
        _prepareMining = prepareMining;
        _prepareAttack = prepareAttack;
        _miningAttack = miningAttack;
        _deadState = deadState;
        _tutorialStunnedState = tutorialStunnedState;
        _stats = stats;

        SubscribeToStates();
    }

    private void SubscribeToStates()
    {
        _disposable = new CompositeDisposable();

        _currentState
            .Subscribe( state => _stats.AIStateName = state == null ? "empty" : state.Name )
            .AddTo( _disposable );

        _idleState.HeroOpeningContainerRequest
            .Subscribe( _ => SetState( _openingContainer ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.PrepareMiningRequest,
                          _meleeAttackState.PrepareMiningRequest,
                          _rangeAttackState.PrepareMiningRequest
            ).Subscribe( _ => SetState( _prepareMining ) )
            .AddTo( _disposable );
        
        /*_idleState.PrepareMiningRequest
            .Subscribe( _ => SetState( _prepareMining ) )
            .AddTo( _disposable );#1#

        _idleState.PrepareAttackRequest
            .Subscribe( _ => SetState( _prepareAttack ) )
            .AddTo( _disposable );

        _prepareMining.HeroMiningRequest
            .Subscribe( _ => SetState( _miningAttack ) )
            .AddTo( _disposable );

        Observable.Merge( _onlyMoveState.HeroIdleRequest,
                          _meleeAttackState.HeroIdleRequest,
                          _rangeAttackState.HeroIdleRequest,
                          _openingContainer.HeroIdleRequest,
                          _prepareAttack.HeroIdleRequest,
                          _prepareMining.HeroIdleRequest,
                          _miningAttack.HeroIdleRequest,
                          _tutorialStunnedState.HeroIdleRequest
                )
            .Subscribe( _ => SetState( _idleState ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.HeroOnlyMoveRequest,
                          _meleeAttackState.HeroOnlyMoveRequest,
                          _rangeAttackState.HeroOnlyMoveRequest,
                          _rangeAttackState.HeroOnlyMoveRequest,
                          _prepareAttack.HeroOnlyMoveRequest,
                          _prepareMining.HeroOnlyMoveRequest,
                          _miningAttack.HeroOnlyMoveRequest
                          )
            .Subscribe( _ => SetState( _onlyMoveState ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.DeadRequest,
                          _onlyMoveState.DeadRequest,
                          _meleeAttackState.DeadRequest,
                          _rangeAttackState.DeadRequest,
                          _rangeAttackState.DeadRequest,
                          _prepareAttack.DeadRequest,
                          _prepareMining.DeadRequest,
                          _miningAttack.DeadRequest
            )
            .Subscribe( _ => SetState( _deadState ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.MeleeAttackRequest,
                          _onlyMoveState.MeleeAttackRequest,
                          _prepareMining.MeleeAttackRequest,
                          _prepareAttack.MeleeAttackRequest
                          )
            .Subscribe( _ => SetState( _meleeAttackState ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.RangeAttackRequest,
                          _onlyMoveState.RangeAttackRequest,
                          _prepareMining.RangeAttackRequest,
                          _prepareAttack.RangeAttackRequest
                          )
            .Subscribe( _ => SetState( _rangeAttackState ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.TutorialStunRequest,
                          _onlyMoveState.TutorialStunRequest,
                          _meleeAttackState.TutorialStunRequest,
                          _miningAttack.TutorialStunRequest
            )
            .Subscribe( _ => SetState( _tutorialStunnedState ) )
            .AddTo( _disposable );

    }*/
    }
}
