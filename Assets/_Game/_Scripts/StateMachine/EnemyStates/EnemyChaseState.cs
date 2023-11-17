namespace _Game._Scripts.StateMachine.EnemyStates
{
    using System.Collections;
    using _Game._Scripts.Character;
    using _Game._Scripts.Character.Enemy;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.StateMachine.Base;
    using _Game._Scripts.Utilities;
    using UnityEngine;

    public class EnemyChaseState : IAIState
    {
        private readonly EnemyAIStateMachine _stateMachine;
        private readonly IEnemyFacade _enemy;
        private readonly CoroutineRunner _coroutineRunner;

        public EnemyChaseState( EnemyAIStateMachine stateMachine, IEnemyFacade enemy, CoroutineRunner coroutineRunner)
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
            _coroutineRunner = coroutineRunner;
        }

        public string Name => "Chase";

        public void Enter()
        {
            _coroutineRunner.Run( ChaseRoutine(), UnitRoutinesID.Chase );
        }

        public void Exit()
        {
            StopChase();
            _enemy.StopMove();
        }

        public void Update()
        {
            DoUpdate();
        }

        public void FixedUpdate()
        {
        }

        private void DoUpdate()
        {
            if ( _enemy.IsDead )
            {
                StopChase();
               _stateMachine.SetState<EnemyDeadState>();
               return;
            }

            var isHeroTarget = _enemy.CurrentTarget is IHeroFacade hero;
            
            if ( isHeroTarget == false )
            {
                StopChase();
                _stateMachine.SetState<EnemyIdleState>();
                return;
            }

            float distanceToTarget = Vector3.Distance( _enemy.Position, _enemy.CurrentTarget.Position );
            float attackDistance = _enemy.Equip.WeaponAttackDistance;
            float additionalDist = 0.2f;

            if ( distanceToTarget + additionalDist <= attackDistance )
            {
               if ( _enemy.Equip.IsActiveWeaponMelee )
                   _stateMachine.SetState<EnemyMeleeAttackState>();
               else if ( _enemy.Equip.IsActiveWeaponRange )
                   _stateMachine.SetState<EnemyRangeAttackState>();
            }
            else if ( distanceToTarget <= 0.1f )
            {
                _stateMachine.SetState<EnemyIdleState>();
            }
            else
            {
                _enemy.TryFollowPath();
            }
        }

        private void StopChase()
        {
            _coroutineRunner.Stop( UnitRoutinesID.Chase );
        }

        private IEnumerator ChaseRoutine()
        {
            var waiter = new WaitForSeconds( 0.01f );
            
            if( _enemy.CurrentTarget != null )
                _enemy.TryPavePath( _enemy.CurrentTarget.Position );
        
            while ( true )
            {
                yield return waiter;
                
                if( _enemy.CurrentTarget != null)
                    _enemy.TryPavePath( _enemy.CurrentTarget.Position );
            }
        }
    }
}
