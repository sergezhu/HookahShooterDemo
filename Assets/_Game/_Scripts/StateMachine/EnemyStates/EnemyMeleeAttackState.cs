using System.Collections;
using _Game._Scripts.StateMachine.Base;
using UniRx;
using UnityEngine;

namespace _Game._Scripts.StateMachine.EnemyStates
{
    using _Game._Scripts.Character;
    using _Game._Scripts.Character.Enemy;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.Utilities;

    public class EnemyMeleeAttackState : IAIState
    {
        private readonly EnemyAIStateMachine _stateMachine;
        private readonly IEnemyFacade _enemy;
        private readonly ICoroutineRunner _coroutineRunner;
        
        private bool _stopAttackFlag;


        public EnemyMeleeAttackState( EnemyAIStateMachine stateMachine, IEnemyFacade enemy, ICoroutineRunner coroutineRunner )
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
            _coroutineRunner = coroutineRunner;
        }

        public string Name => "MeleeAttack";

        public void Enter()
        {
            Debug.Log( $"Enter - melee attack" );
            StartAttack(); 
        }

        public void Exit()
        {
            Debug.Log( $"Exit - melee attack" );
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
            
            if ( _enemy.CurrentTarget == null || _enemy.CurrentTarget is IHeroFacade hero == false)
            {
                StopAttack();
                _stateMachine.SetState<EnemyIdleState>();
                return;
            }
        
            _enemy.RotateToPosition(_enemy.CurrentTarget.Position);

            float distanceToTarget = Vector3.Distance( _enemy.Position, _enemy.CurrentTarget.Position );
            float attackDistance = _enemy.Equip.WeaponAttackDistance;

            _stopAttackFlag = distanceToTarget > attackDistance;
        }

        public void FixedUpdate()
        {
        }

        private void StartAttack()
        {
            _stopAttackFlag = false;
            _coroutineRunner.Run( AttackRoutine(), UnitRoutinesID.Attack );
        }

        private void StopAttack()
        {
            _coroutineRunner.Stop( UnitRoutinesID.Attack );
            _enemy.StopAttack();
        }

        private IEnumerator AttackRoutine()
        {
            if ( _enemy.IsAttackRollbackDoing )
            {
                _stateMachine.SetState<EnemyIdleState>();
                yield break;
            }
            
            _enemy.TryStartMeleeAttack();
        
            while ( true )
            {
                if ( _stopAttackFlag )
                {
                    StopAttack();
                    _stateMachine.SetState<EnemyChaseState>();
                }
                
                yield return new WaitForSeconds(_enemy.Equip.WeaponAttackDelay );

                if ( _enemy.IsAttackRollbackDoing )
                {
                    _stateMachine.SetState<EnemyIdleState>();
                    yield break;
                }
                
                _enemy.TryStartMeleeAttack();
            }
        }
    }
}
