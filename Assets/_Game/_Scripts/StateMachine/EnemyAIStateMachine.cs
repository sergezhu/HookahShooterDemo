namespace _Game._Scripts.StateMachine
{
    using _Game._Scripts.StateMachine.Base;

    public class EnemyAIStateMachine : BaseAIStateMachine
    {
        /*private CompositeDisposable _disposable;

    private readonly EnemyIdleState _idleState;
    private readonly EnemyPatrolState _patrolState;
    private readonly EnemyMovingToTargetState _movingToTargetState;
    private readonly EnemyMoveToTargetPreparingState _moveToTargetPreparingState;
    private readonly EnemyChaseState _chaseState;
    private readonly EnemyDeadState _deadState;
    private readonly EnemyMeleeAttackState _meleeAttackState;
    private readonly EnemyRangeAttackState _rangeAttackState;

    public EnemyAIStateMachine(EnemyIdleState idle, EnemyPatrolState patrol, EnemyMovingToTargetState movingToTargetState, EnemyMoveToTargetPreparingState moveToTargetPreparingState, 
                               EnemyChaseState chase, EnemyDeadState deadState, EnemyMeleeAttackState meleeAttack, EnemyRangeAttackState rangeAttack ) : base()
    {
        _states[typeof(EnemyIdleState)] = idle;
        _states[typeof(EnemyPatrolState)] = patrol;
        _states[typeof(EnemyMovingToTargetState)] = movingToTargetState;
        _states[typeof(EnemyMoveToTargetPreparingState)] = moveToTargetPreparingState;
        _states[typeof(EnemyChaseState)] = chase;
        _states[typeof(EnemyDeadState)] = deadState;
        _states[typeof(EnemyMeleeAttackState)] = meleeAttack;
        _states[typeof(EnemyRangeAttackState)] = rangeAttack;

        _idleState = idle;
        _patrolState = patrol;
        _movingToTargetState = movingToTargetState;
        _moveToTargetPreparingState = moveToTargetPreparingState;
        _chaseState = chase;
        _deadState = deadState;
        _meleeAttackState = meleeAttack;
        _rangeAttackState = rangeAttack;

        SubscribeToStates();
    }

    private void SubscribeToStates()
    {
        _disposable = new CompositeDisposable();

        _currentState
            .Subscribe( state => _stats.AIStateName = state == null ? "empty" : state.Name )
            .AddTo( _disposable );

        _idleState.MoveToTargetPreparingStateRequest
            .Subscribe( _ => SetState( _moveToTargetPreparingState ) )
            .AddTo( _disposable );
        
        _moveToTargetPreparingState.MoveToTargetStateRequest
            .Subscribe( _ => SetState( _movingToTargetState ) )
            .AddTo( _disposable );

        _chaseState.MeleeAttackStateRequest
            .Subscribe( _ => SetState( _meleeAttackState ) )
            .AddTo( _disposable );

        _chaseState.RangeAttackStateRequest
            .Subscribe( _ => SetState( _rangeAttackState ) )
            .AddTo( _disposable );

        Observable.Merge( _chaseState.IdleStateRequest, 
                          _meleeAttackState.IdleStateRequest, 
                          _rangeAttackState.IdleStateRequest,
                          _movingToTargetState.IdleStateRequest,
                          _moveToTargetPreparingState.IdleStateRequest
                )
            .Subscribe( _ => SetState( _idleState ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.ChaseStateRequest, 
                          _meleeAttackState.ChaseStateRequest, 
                          _rangeAttackState.ChaseStateRequest,
                          _movingToTargetState.ChaseStateRequest,
                          _moveToTargetPreparingState.ChaseStateRequest )
            .Subscribe( _ => SetState( _chaseState ) )
            .AddTo( _disposable );

        Observable.Merge( _idleState.DeadRequest,
                          _chaseState.DeadRequest, 
                          _patrolState.DeadRequest, 
                          _meleeAttackState.DeadRequest,
                          _rangeAttackState.DeadRequest,
                          _movingToTargetState.DeadRequest,
                          _moveToTargetPreparingState.DeadRequest )
            .Subscribe( _ => SetState( _deadState ) )
            .AddTo( _disposable );
    }

    public void CleanUp()
    {
        base.CleanUp();
        _disposable.Clear();
    }*/
    }
}
