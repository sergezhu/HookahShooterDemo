namespace _Game._Scripts.StateMachine.PlayerStates
{
    using System.Collections;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.Character.Unit;
    using _Game._Scripts.StateMachine.Base;
    using _Game._Scripts.UI.Hud;
    using _Game._Scripts.Utilities;
    using UnityEngine;

    public class PlayerMeleeAttackState : IAIState
    {
        private readonly HeroAIStateMachine _stateMachine;
        private readonly UnitAttackSystem _attackSystem;
        private readonly IHeroFacade _hero;
        private readonly ICoroutineRunner _coroutineRunner;


        public PlayerMeleeAttackState( HeroAIStateMachine stateMachine, UnitAttackSystem attackSystem, IHeroFacade hero, ICoroutineRunner coroutineRunner )
        {
            _stateMachine = stateMachine;
            _attackSystem = attackSystem;
            _hero = hero;
            _coroutineRunner = coroutineRunner;
        }

        public string Name => "MeleeAttack";
    
        public void Enter()
        {
            _hero.StopMove();
            
            StartAttack();
        }

        public void Exit()
        {
            StopAttack();
        }

        public void Update()
        {
            if ( _hero.IsDead )
            {
                StopAttack();
                _stateMachine.SetState<PlayerDeadState>();
                return;
            }

            if ( _hero.IsMeleeAttackActive.Value == false )
            {
                StopAttack();
                _stateMachine.SetState<PlayerIdleState>();
                return;
            }

            if ( _hero.CurrentTarget == null )
            {
                _stateMachine.SetState<PlayerIdleState>();
                return;
            }

            if ( _hero.IsInputMoving )
            {
                StopAttack();
                _stateMachine.SetState<PlayerOnlyMoveState>();
                return;
            }

            var target = _hero.CurrentTarget;
            _hero.RotateToPosition( target.Position );

            float distanceToTarget = Vector3.Distance( _hero.Position, target.Position );
            float attackDistance = _hero.Equip.WeaponAttackDistance;

            if ( distanceToTarget > attackDistance )
            {
                StopAttack();
                _stateMachine.SetState<PlayerIdleState>();
            }
            else
            {
                //Debug.Log( $"Attacking on {target.Name} in attack range" );
            }
        }

        public void FixedUpdate()
        {
        }

        private void StopAttack()
        {
            _coroutineRunner.Stop( 1 );
            _hero.StopAttack();
        }

        private void StartAttack()
        {
            _coroutineRunner.Run( AttackRoutine(), 1 );
        }

        private IEnumerator AttackRoutine()
        {
            if ( _hero.IsAttackRollbackDoing )
            {
                _stateMachine.SetState<PlayerIdleState>();
                yield break;
            }
            
            _hero.TryRunMeleeAttack();

            while ( true )
            {
                yield return new WaitForSeconds( _hero.Equip.WeaponAttackDelay );

                if ( _hero.IsAttackRollbackDoing )
                {
                    _stateMachine.SetState<PlayerIdleState>();
                    yield break;
                }
                
                _hero.TryRunMeleeAttack();
            }
        }
    }
}
