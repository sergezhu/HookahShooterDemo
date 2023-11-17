namespace _Game._Scripts.StateMachine.EnemyStates
{
    using _Game._Scripts.Character.Enemy;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.StateMachine.Base;
    using _Game._Scripts.Utilities.Extensions;
    using UnityEngine;

    public class EnemyIdleState : IAIState
    {
        private readonly EnemyAIStateMachine _stateMachine;
        private readonly IEnemyFacade _enemy;

        public EnemyIdleState( EnemyAIStateMachine stateMachine, IEnemyFacade enemy )
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
        }

        public string Name => "Idle";
    
        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if ( _enemy.IsDead )
            {
                _stateMachine.SetState<EnemyDeadState>();
                return;
            }

            if ( _enemy.CurrentTarget == null )
                return;

            if ( _enemy.CurrentTarget is IHeroFacade )
            {
                float sqrDistanceToTarget = Vector3.SqrMagnitude( (_enemy.Position - _enemy.CurrentTarget.Position).WithY( 0 ) );

                if ( sqrDistanceToTarget < _enemy.FieldOfViewDistance * _enemy.FieldOfViewDistance || _enemy.IsAggro )
                {
                    _stateMachine.SetState<EnemyChaseState>();
                    return;
                }
            }
        }

        public void FixedUpdate()
        {
        }
    }
}
