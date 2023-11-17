namespace _Game._Scripts.StateMachine.PlayerStates
{
    using System.Collections;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.Character.Unit;
    using _Game._Scripts.Configs;
    using _Game._Scripts.Enums;
    using _Game._Scripts.Interfaces;
    using _Game._Scripts.StateMachine.Base;
    using _Game._Scripts.Utilities;
    using UniRx;
    using UnityEngine;

    public class PlayerRangeAttackState : IAIState
    {
        private readonly HeroAIStateMachine _stateMachine;
        private readonly UnitAttackSystem _attackSystem;
        private readonly IHeroFacade _hero;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly HeroConfig _heroConfig;


        public PlayerRangeAttackState( HeroAIStateMachine stateMachine, UnitAttackSystem attackSystem, IHeroFacade hero, ICoroutineRunner coroutineRunner, HeroConfig heroConfig )
        {
            _stateMachine = stateMachine;
            _attackSystem = attackSystem;
            _hero = hero;
            _coroutineRunner = coroutineRunner;
            _heroConfig = heroConfig;
        }

        public string Name => "RangeAttack";
    
        public void Enter() 
        {
            //Debug.Log( $"Range Attack - Enter" );
            
            if(_heroConfig.IsShootingWhenMovingEnabled == false)
                _hero.StopMove();

            var target = _hero.CurrentTarget;
            _hero.RotateToPosition( target.Position, true );
            
            StartAttack();
        }

        public void Exit()
        {
            //Debug.Log( $"Range Attack - Exit" );

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
            
            if ( _hero.CurrentTarget == null )
            {
                _stateMachine.SetState<PlayerIdleState>();
                return;
            }

            if ( _hero.IsInputMoving )
            {
                if ( _heroConfig.IsShootingWhenMovingEnabled )
                {
                    _hero.TryInputMove( false );
                }
                else
                {
                    StopAttack();
                    _stateMachine.SetState<PlayerIdleState>();
                    return;
                }
            }
            else
            {
                _hero.StopMove();
            }

            if ( _hero.IsRangeAttackActive.Value == false )
            {
                StopAttack();

                if ( _hero.IsInputMoving && _heroConfig.IsShootingWhenMovingEnabled )
                    _stateMachine.SetState<PlayerOnlyMoveState>();
                else
                    _stateMachine.SetState<PlayerIdleState>();
                
                return;
            }

            var target = _hero.CurrentTarget;
            _hero.RotateToPosition( target.Position, true );

            float distanceToTarget = Vector3.Distance( _hero.Position, target.Position );
            float attackDistance = _hero.Equip.WeaponAttackDistance;

            if ( distanceToTarget > attackDistance )
            {
                StopAttack();
                _stateMachine.SetState<PlayerIdleState>();
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
            var attackDelay = _hero.Equip.WeaponAttackDelay;
            var waitAttackDelay = new WaitForSeconds( attackDelay );
            Debug.Log($"StartRangeAttackRoutine : {attackDelay}"  );
            
            if ( _hero.IsAttackRollbackDoing )
            {
                _stateMachine.SetState<PlayerIdleState>();
                yield break;
            }
            
            _hero.TryRangeAttack();

            while ( true )
            {
                yield return waitAttackDelay;

                if ( _hero.IsAttackRollbackDoing )
                {
                    _stateMachine.SetState<PlayerIdleState>();
                    yield break;
                }
                
                _hero.TryRangeAttack();
            }
        }
    }
}
