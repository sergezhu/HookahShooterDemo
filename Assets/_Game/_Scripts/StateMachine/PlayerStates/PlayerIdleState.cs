namespace _Game._Scripts.StateMachine.PlayerStates
{
    using _Game._Scripts.Character.Enemy;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.Character.Unit;
    using _Game._Scripts.StateMachine.Base;
    using UniRx;
    using UnityEngine;

    public class PlayerIdleState : IAIState
    {
        private readonly HeroAIStateMachine _stateMachine;
        private readonly UnitAttackSystem _attackSystem;
        private readonly IHeroFacade _hero;
        private readonly CompositeDisposable _disposable;


        public PlayerIdleState( HeroAIStateMachine stateMachine, UnitAttackSystem attackSystem, IHeroFacade hero )
        {
            _stateMachine = stateMachine;
            _attackSystem = attackSystem;
            _hero = hero;
            
            _disposable = new CompositeDisposable();    
        }

        public string Name => "Idle";

    
        public void Enter()
        {
            Debug.Log( "Idle - Enter" );
            
            _hero.StopMove();
        }

        private void TryAim()
        {
            var enemy = _hero.CurrentTarget as IEnemyFacade;

            if ( enemy != null && _hero.Equip.HasWeapon )
            {
                float sqrDistanceToEnemy = Vector3.SqrMagnitude( _hero.Position - enemy.Position );

                if ( sqrDistanceToEnemy < _hero.Equip.WeaponAttackDistance * _hero.Equip.WeaponAttackDistance )
                {
                    _hero.RotateToPosition( enemy.Position, true );
                }
            }
        }

        public void Exit()
        {
            Debug.Log( "Idle - Exit" );
            _disposable.Clear();
        }

        public void Update()
        {
            if ( _hero.IsDead )
            {
                _stateMachine.SetState<PlayerDeadState>();
                return;
            }
            
            if ( _hero.IsInputMoving )
            {
                _stateMachine.SetState<PlayerOnlyMoveState>();
                return;
            }

            TryAim();

            if ( TryRequestAttack() )
                return;
        }

        private bool TryRequestAttack()
        {
            if ( _hero.CurrentTarget != null && _hero.IsAttackRollbackDoing == false )
            {
                //Debug.Log( $"TryRequestAttack 2, melee {_hero.IsMeleeAttackActive.Value}, range {_hero.IsRangeAttackActive.Value}" );

                if ( _hero.CurrentTarget is IEnemyFacade && _hero.IsMeleeAttackActive.Value )
                {
                    if ( CanStartWithMelee() )
                    {
                        _stateMachine.SetState<PlayerMeleeAttackState>();
                        return true;
                    }
                }

                if ( _hero.CurrentTarget is IEnemyFacade && _hero.IsRangeAttackActive.Value )
                {
                    if ( CanStartWithRange() )
                    {
                        _stateMachine.SetState<PlayerRangeAttackState>();
                        return true;
                    }
                }
            }

            return false;
        }

        private bool InRange( Vector3 pos1, Vector3 pos2, float maxDistance )
        {
            float distanceToTarget = Vector3.Distance( pos1, pos2 );
            return distanceToTarget < maxDistance;
        }

        private bool CanStartWithMelee()
        {
            var target = _hero.CurrentTarget;
            return InRange( _hero.Position, target.Position, _hero.Equip.WeaponAttackDistance );
        }

        private bool CanStartWithRange()
        {
            var target = _hero.CurrentTarget;
            return InRange( _hero.Position, target.Position, _hero.Equip.WeaponAttackDistance );
        }

        public void FixedUpdate()
        {
        }
    }
}
