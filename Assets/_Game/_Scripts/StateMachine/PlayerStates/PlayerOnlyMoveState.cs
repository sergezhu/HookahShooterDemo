namespace _Game._Scripts.StateMachine.PlayerStates
{
    using _Game._Scripts.Character.Enemy;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.Character.Unit;
    using _Game._Scripts.Configs;
    using _Game._Scripts.StateMachine.Base;
    using UnityEngine;

    public class PlayerOnlyMoveState : IAIState
    {
        private readonly HeroAIStateMachine _stateMachine;
        private readonly UnitAttackSystem _attackSystem;
        private readonly IHeroFacade _hero;
        private readonly HeroConfig _heroConfig;


        public PlayerOnlyMoveState( HeroAIStateMachine stateMachine, UnitAttackSystem attackSystem, IHeroFacade hero, HeroConfig heroConfig )
        {
            _stateMachine = stateMachine;
            _attackSystem = attackSystem;
            _hero = hero;
            _heroConfig = heroConfig;
        }

        public string Name => "OnlyMove";
    
        public void Enter()
        {
            Debug.Log( "OnlyMove - Enter" );
        }

        public void Exit()
        {
            Debug.Log( "OnlyMove - Exit" );
        }

        public void Update()
        {
            if ( _hero.IsDead )
            {
                _stateMachine.SetState<PlayerDeadState>();
                return;
            }
            
            if ( _hero.IsInputMoving == false )
            {
                _stateMachine.SetState<PlayerIdleState>();
                return;
            }

            bool isAiming = false;
            
            if ( _heroConfig.IsShootingWhenMovingEnabled )
            {
                bool isAttackSuccess = TryEnemyRequestAttack();
                
                isAiming = TryAim();

                if ( isAttackSuccess )
                    return;
            }

            bool needRotate = isAiming == false || _heroConfig.IsShootingWhenMovingEnabled == false;
            
            _hero.TryInputMove( needRotate );
        }

        private bool TryAim()
        {
            if ( _hero.CurrentTarget is IEnemyFacade )
            {
                if ( _hero.Equip.IsActiveWeaponRange )
                {
                    var target = _hero.CurrentTarget;
                    float sqrDistanceToTarget = Vector3.SqrMagnitude( _hero.Position - target.Position );
                    var attackDistance = _hero.Equip.WeaponAttackDistance;
                    
                    if ( sqrDistanceToTarget < attackDistance * attackDistance )
                    {
                        _hero.RotateToPosition( _hero.CurrentTarget.Position, true );
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryEnemyRequestAttack()
        {
            if ( _hero.CurrentTarget is IEnemyFacade && _hero.IsAttackRollbackDoing == false )
            {
                if ( _hero.IsRangeAttackActive.Value )
                {
                    if ( TryStartRangeAttack() )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryStartRangeAttack()
        {
            var target = _hero.CurrentTarget;
            float sqrDistanceToTarget = Vector3.SqrMagnitude( _hero.Position - target.Position );
            var attackDistance = _hero.Equip.WeaponAttackDistance;

            if ( sqrDistanceToTarget < attackDistance * attackDistance )
            {
                _stateMachine.SetState<PlayerRangeAttackState>();
                return true;
            }

            return false;
        }

        public void FixedUpdate()
        {
        }
    }
}
