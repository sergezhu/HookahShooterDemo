namespace _Game._Scripts.Character.Hero
{
    using _Game._Scripts.Character.Animations;
    using _Game._Scripts.Character.General.Movement;
    using _Game._Scripts.Interfaces;
    using _Game._Scripts.Level;
    using _Game._Scripts.Managers.LevelsManagement;
    using _Game._Scripts.StateMachine;
    using _Game._Scripts.StateMachine.Base;
    using _Game._Scripts.StateMachine.PlayerStates;
    using UnityEngine;
    using UnityEngine.AI;
    using Zenject;

    public sealed class HeroAIController : IInitializable
    {
        private readonly IHeroView _view;
        private readonly HeroAIStateMachine _heroAIStateMachine;
        private readonly AIStateFactory _stateFactory;
        private readonly CharacterMovement _movement;
        private readonly HeroLiveState _liveState;
        private readonly UnitAnimatorController _unitAnimatorController;
        private readonly NavMeshProvider _navProvider;

        private LevelTargetsProvider _targetsProvider;
        private bool _activated;
        private bool _lockedStateStored;
        private readonly NavMeshPath _path;

        public Vector3 Position => _view.Position;

        public bool IsActive => _activated;
        public bool AiLocked { get; private set; }
        public bool IsPaused { get; private set; }


        private ITarget CurrentDamageableTarget { set; get; }

        //private float Speed => _moveConfig.MeshAgentSpeed;
        private float ConfigMoveSpeed => _movement.MoveSpeed;
        private float CurrentMoveSpeed => _movement.CurrentSpeed;
        private float RotationSpeed => _movement.RotationSpeed;


        public HeroAIController( IHeroView view, HeroAIStateMachine heroAIStateMachine, AIStateFactory stateFactory, CharacterMovement movement
                                 , HeroLiveState liveState, UnitAnimatorController unitAnimatorController, NavMeshProvider navProvider)
        {
            _view = view;
            _heroAIStateMachine = heroAIStateMachine;
            _stateFactory = stateFactory;
            _movement = movement;
            _liveState = liveState;
            _unitAnimatorController = unitAnimatorController;
            _navProvider = navProvider;

            _path = new NavMeshPath();

            _activated = false;
        }

        public void Initialize()
        {
            InitializeFSM();
        }

        public void Activate()
        {
            if ( _activated )
                return;
            
            _activated = true;

            AttachToNavmesh();

            _heroAIStateMachine.Start();
        }

        public void Deactivate()
        {
            if(!_activated)
                return;
            
            _activated = false;
            _heroAIStateMachine.Stop();
        }

        public void AttachTargetsProvider( LevelTargetsProvider targetsProvider )
        {
            _targetsProvider = targetsProvider;
        }

        public void DetachTargetsProvider()
        {
            _targetsProvider = null;
        }

        public void Tick()
        {
            if( !_activated || _targetsProvider == null)
                return;

            UpdateAnimator();

            _heroAIStateMachine.Tick();
        }

        public void TryNavmeshMoveAlongDir( Vector3 dir, bool needRotate )
        {
            var offset = 0;
            
            dir.Normalize();
            var currentPos = _view.Position;
            
            var nextPos = _movement.GetNextPoint( currentPos, dir, offset );
            var dist = _movement.SamplePositionDistance;

            NavMeshData navMeshData = _navProvider.CurrentNavSurface.navMeshData;

            if ( navMeshData != null )
            {
                var color = "green";
                var isNextPosOnNavmesh = NavMesh.SamplePosition( nextPos, out var hit, dist, NavMesh.AllAreas );
                //Debug.Log( $"From {currentPos} to {nextPos}. Is nextPos on nav surface: {isNextPosOnNavmesh} : {Time.frameCount}" );
                var vector = hit.position - currentPos;

                if ( isNextPosOnNavmesh )
                {
                    // Debug.Log( $"Hit {hit.position} : {Time.frameCount}" );

                    if ( vector.magnitude >= 0.02f )
                    {
                        _movement.MoveTo( hit.position, needRotate );
                    }
                    else
                    {
                        //_movement.StopMove();
                        color = "yellow";
                    }
                }
                else
                {
                    //_movement.StopMove();
                    color = "red";
                }
                
                Debug.Log( $"<color={color}>Try move from {currentPos} to {currentPos + dir}</color>" );
            }
        }

        public void MoveAlongDir( Vector3 dir, bool needRotate )
        {
            _movement.MoveAlongDir( dir, needRotate );
        }

        public void StopMove()
        {
            _movement.StopMove();
        }

        public void RotateToPosition( Vector3 position, bool force = false )
        {
            Vector3 directionToPosition = GetDirectionToPosition( position );
            directionToPosition.y = 0;

            if(force)
                _movement.Rotate( directionToPosition, 10000 );
            else
                _movement.Rotate( directionToPosition );
        }

        public void RotateToPosition( Vector3 position, float additionalRotateAngleY, bool force = false )
        {
            Vector3 directionToPosition = GetDirectionToPosition( position );
            directionToPosition.y = 0;
            
            Quaternion additionalRotate = Quaternion.Euler( 0, additionalRotateAngleY, 0 );
            directionToPosition = additionalRotate * directionToPosition;

            if(force)
                _movement.Rotate( directionToPosition, 10000 );
            else
                _movement.Rotate( directionToPosition );
        }

        public Vector3 GetDirectionToPosition( Vector3 targetPosition )
        {
            return (targetPosition - _view.Position).normalized;
        }

        public void CleanUp()
        {
            Deactivate();
            CleanUpFSM();
        }

        public void Pause()
        {
            if ( IsPaused || !_activated )
                return;

            IsPaused = true;
        }

        public void Release()
        {
            if ( !IsPaused || !_activated )
                return;

            IsPaused = false;
        }

        public void SetDamageableTarget( IDamageableTarget currentDamageableTarget )
        {
            CurrentDamageableTarget = currentDamageableTarget;
        }

        private void InitializeFSM()
        {
            _heroAIStateMachine.AddState( _stateFactory.CreateState<PlayerIdleState>() );
            _heroAIStateMachine.AddState( _stateFactory.CreateState<PlayerOnlyMoveState>() );
            _heroAIStateMachine.AddState( _stateFactory.CreateState<PlayerMeleeAttackState>() );
            _heroAIStateMachine.AddState( _stateFactory.CreateState<PlayerRangeAttackState>() );
            _heroAIStateMachine.AddState( _stateFactory.CreateState<PlayerDeadState>() );
            
            _heroAIStateMachine.SetState<PlayerIdleState>();
        }

        private void CleanUpFSM()
        {
            _heroAIStateMachine.CleanUp();
        }

        private void AttachToNavmesh()
        {
            NavMesh.SamplePosition( _view.Position, out var hit, 1f, NavMesh.AllAreas );
            _movement.MoveTo( hit.position, false );
        }

        private void UpdateAnimator()
        {
            _unitAnimatorController.SetVelocity( _movement.CurrentVelocity );
        }
    }
}