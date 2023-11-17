using System.Collections;
using _Game._Scripts.StateMachine.Base;
using UniRx;
using UnityEngine;

namespace _Game._Scripts.StateMachine.EnemyStates
{
    using _Game._Scripts.Character;
    using _Game._Scripts.Character.Enemy;
    using _Game._Scripts.Utilities;

    public class EnemyRangeAttackState : IAIState
    {
        private readonly EnemyAIStateMachine _stateMachine;
        private readonly IEnemyFacade _enemy;
        private readonly ICoroutineRunner _coroutineRunner;

    
        public EnemyRangeAttackState( EnemyAIStateMachine stateMachine, IEnemyFacade enemy, ICoroutineRunner coroutineRunner )
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
            _coroutineRunner = coroutineRunner;
        }

        public string Name => "RangeState";

        public void Enter()
        {
            StartAttack();
        }

        public void Exit()
        {
            StopAttack();
        }

        public void Update()
        {
            if ( _enemy.IsDead )
            {
                StopAttack();
                _stateMachine.SetState<EnemyDeadState>();
                return;
            }
            
            if ( _enemy.CurrentTarget == null )
            {
                _stateMachine.SetState<EnemyIdleState>();
                return;
            }

            _enemy.RotateToPosition( _enemy.CurrentTarget.Position );

            float distanceToTarget = Vector3.Distance( _enemy.Position, _enemy.CurrentTarget.Position );
            float attackDistance = _enemy.Equip.WeaponAttackDistance;

            if ( distanceToTarget > attackDistance )
            {
                StopAttack();
                _stateMachine.SetState<EnemyChaseState>();
            }
        }

        public void FixedUpdate()
        {
        }

        private void StartAttack()
        {
            _coroutineRunner.Run( AttackRoutine(), UnitRoutinesID.Attack );
        }

        private void StopAttack()
        {
            _coroutineRunner.Stop( UnitRoutinesID.Attack );
            _enemy.StopAttack();
        }

        private IEnumerator AttackRoutine()
        {
            if ( _enemy.IsAttackRollbackDoing || _enemy.CurrentTarget == null )
            {
                _stateMachine.SetState<EnemyIdleState>();
                yield break;
            }
            
            _enemy.TryStartRangeAttack();
            
            while ( true )
            {
                yield return new WaitForSeconds( _enemy.Equip.WeaponAttackDelay );

                if ( _enemy.IsAttackRollbackDoing || _enemy.CurrentTarget == null )
                {
                    _stateMachine.SetState<EnemyIdleState>();
                    yield break;
                }
                
                _enemy.TryStartRangeAttack();
            }
        }
    }
}