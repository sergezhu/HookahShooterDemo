namespace _Game._Scripts.Character.Enemy
{
    using _Game._Scripts.Character.General.Movement;
    using _Game._Scripts.Configs;
    using _Game._Scripts.Interfaces;
    using _Game._Scripts.Level;
    using _Game._Scripts.Managers.LevelsManagement;
    using _Game._Scripts.StateMachine;
    using _Game._Scripts.StateMachine.Base;
    using _Game._Scripts.StateMachine.EnemyStates;
    using _Game._Scripts.Utilities.Timers;
    using DG.Tweening;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AI;
    using Zenject;

    public sealed class EnemyAIController : IInitializable
    {
        private readonly EnemyView _view;
        private readonly EnemyAIStateMachine _enemyAIStateMachine;
        private readonly AIStateFactory _stateFactory;
        private readonly CharacterMovement _movement;
        private readonly LevelTargetsProvider _targetsProvider;
        private readonly EnemyConfig _enemyConfig;
        private readonly AIConfig _aiConfig;
        private readonly NavMeshProvider _navProvider;
        private readonly SimpleTimer _chasingTimer;
        private readonly SimpleTimer _aggroTimer;


        private bool _lockedStateStored;
        private Tween _attackRollbackTween;
        private bool _chasingFlagIfOutRange;

        public Vector3 Position => _view.Position;
        public Vector3 Direction => _view.Forward;

        public bool IsActive => IsActivated.Value;
        public bool IsPaused { get; private set; }
        public bool AiLocked { get; private set; }
        public ITarget CurrentTarget { private set; get; }
        public ITarget NearestDamageableTarget { private set; get; }
        public bool IsAggro => _aggroTimer.State == TimerState.Running;

        public ReactiveCommand<float> RollbackProgressChanged { get; } = new ReactiveCommand<float>();
        private ReactiveProperty<bool> IsActivated { get; } = new ReactiveProperty<bool>( false );


        //private float Speed => _moveConfig.MeshAgentSpeed;
        private float ConfigMoveSpeed => _movement.MoveSpeed;
        private float CurrentMoveSpeed => _movement.CurrentSpeed;
        private float RotationSpeed => _movement.RotationSpeed;


        public EnemyAIController( EnemyView view, EnemyAIStateMachine enemyAIStateMachine, AIStateFactory stateFactory, CharacterMovement movement, LevelTargetsProvider targetsProvider, 
                                  EnemyConfig enemyConfig, AIConfig aiConfig, NavMeshProvider navProvider )
        {
            _view = view;
            _enemyAIStateMachine = enemyAIStateMachine;
            _stateFactory = stateFactory;
            _movement = movement;
            _targetsProvider = targetsProvider;
            _enemyConfig = enemyConfig;
            _aiConfig = aiConfig;
            _navProvider = navProvider;

            _chasingTimer = new SimpleTimer( _aiConfig.ChaseDurationWhenTargetOutOfSight );
            _aggroTimer = new SimpleTimer( _aiConfig.AggroDurationWhenDamaged );
        }

        public void Initialize()
        {
            InitializeFSM();
        }

        public void Activate()
        {
            if ( IsActivated.Value )
                return;
            
            IsActivated.Value = true;
            _view.NavMeshAgent.enabled = true;
            _movement.Activate();
            _enemyAIStateMachine.Start();
        }

        public void Deactivate()
        {
            if(IsActivated.Value == false)
                return;
            
            IsActivated.Value = false;
            
            _movement.Deactivate();
            _enemyAIStateMachine.Stop();
            _aggroTimer.Reset();
            _chasingTimer.Reset();

            _attackRollbackTween.Kill();
        }

        public void Tick()
        {
            //Debug.Log( $"Tick [{_view.SiblingIndex}]{_view.Name} : {IsActivated.Value}" );
            
            if( IsActivated.Value == false )
                return;
            
            _chasingTimer.Tick( Time.deltaTime );
            _aggroTimer.Tick( Time.deltaTime );

            //NearestTarget = _targetsProvider.FindNearestTargetsFrom(_view.Position, ETargetFilter.LiveHero);
            NearestDamageableTarget = _targetsProvider.LiveHeroFacade;

            if ( NearestDamageableTarget != null )
            {
                var vectorToNearestTarget = NearestDamageableTarget.Position - Position;
                var inRange = vectorToNearestTarget.sqrMagnitude <= _enemyConfig.FieldOfViewDistance * _enemyConfig.FieldOfViewDistance;

                if ( inRange )
                {
                    _chasingFlagIfOutRange = true;
                }
                else
                {
                    if ( _chasingFlagIfOutRange )
                    {
                        _chasingTimer.Reset();
                        _chasingTimer.Start();

                        _chasingFlagIfOutRange = false;
                    }
                }

                //if( _aggroTimer.State == TimerState.Running )
                //    Debug.Log( $"[Timer] chasing : {_chasingTimer.State}, aggro : {_aggroTimer.State}" );
                
                var canSelectHero = inRange || _chasingTimer.State == TimerState.Running || _aggroTimer.State == TimerState.Running;
                CurrentTarget = canSelectHero ? NearestDamageableTarget : null;
            }
            else
            {
                CurrentTarget = null;
            }

            UpdateAnimator();

            _enemyAIStateMachine.Tick();
        }


        public void CleanUp()
        {
            Deactivate();
            CleanUpFSM();
        }

        public void Lock()
        {
            if ( !IsActivated.Value )
                return;

            AiLocked = true;
            _enemyAIStateMachine.Stop();
        }

        public void Unlock()
        {
            if ( !IsActivated.Value )
                return;

            AiLocked = false;
            _enemyAIStateMachine.Start();
        }

        public void EnableAggro()
        {
            _aggroTimer.Reset();
            _aggroTimer.Start();
        }

        public void DisableAggro()
        {
            _aggroTimer.Complete(false);
        }

        public void TryPavePath( Vector3 pos )
        {
            if ( _view.NavMeshAgent.IsPathCompleted || _view.NavMeshAgent.IsPathExist == false )
                _view.NavMeshAgent.PavePathToPoint( pos );
        }

        public void TryFollowPath()
        {
            var isPathExist = _view.NavMeshAgent.IsPathExist;
            
            //Debug.Log( $"[Enemy] {_view.Name} : TryFollowPath : isPathExist = {isPathExist}" );

            if ( isPathExist == false )
            {
                StopMove();
                return;
            }

            float distanceToCurrentCorner = Vector3.Distance( _view.Position, _view.NavMeshAgent.CurrentPathCorner );

            if ( distanceToCurrentCorner > _view.NavMeshAgent.StopDistance )
            {
                //MoveToPosition( _view.NavMeshAgent.CurrentPathCorner );
                TryNavmeshMoveTo( _view.NavMeshAgent.CurrentPathCorner, true );
            }
            else
            {
                if ( _view.NavMeshAgent.TryIncreaseCurrentCornerIndex() )
                {
                    TryNavmeshMoveTo( _view.NavMeshAgent.CurrentPathCorner, true );
                }
            }
        }

        public void MoveToPosition( Vector3 position )
        {
            RotateToPosition( position );

            Vector3 direction = GetDirectionTo( position );
            Vector3 nextPos = _movement.GetNextPoint( _view.Position, direction, 0 );
            //directionToPosition.y = 0;

            //_movement.MoveAlongDir( directionToPosition, false );
            _movement.MoveTo( nextPos );
        }

        public void TryNavmeshMoveTo( Vector3 to, bool needRotate )
        {
            var offset = 0;

            var currentPos = _view.Position;
            Vector3 dir = GetDirectionTo( to );
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

                //Debug.Log( $"<color={color}>Try move from {currentPos} to {currentPos + dir}</color>" );
            }
            else
            {
                //Debug.Log( $"<color=red>Try move : navMeshData is null</color>" );
            }
        }

        public void StopMove()
        {
            _movement.StopMove();
        }

        public void RotateToPosition( Vector3 position )
        {
            Vector3 directionToPosition = GetDirectionTo( position );
            directionToPosition.y = 0;

            _movement.Rotate( directionToPosition );
        }

        public void RotateToPosition( Vector3 position, float additionalRotateAngleY, bool force = false )
        {
            Vector3 directionToPosition = GetDirectionTo( position );
            directionToPosition.y = 0;

            Quaternion additionalRotate = Quaternion.Euler( 0, additionalRotateAngleY, 0 );
            directionToPosition = additionalRotate * directionToPosition;

            if ( force )
                _movement.Rotate( directionToPosition, 10000 );
            else
                _movement.Rotate( directionToPosition );
        }

        public Vector3 GetDirectionTo( Vector3 targetPosition )
        {
            return (targetPosition - _view.Position).normalized;
        }

        public float GetAngleToDirection( Vector3 direction )
        {
            Quaternion rotationInDirection = Quaternion.LookRotation( direction );
            return Quaternion.Angle( _view.Rotation, rotationInDirection );
        }

        public void Pause()
        {
            if ( IsPaused || !IsActivated.Value )
                return;

            IsPaused = true;
            _lockedStateStored = AiLocked;

            if ( _lockedStateStored )
                return;

            Lock();
        }

        public void Release()
        {
            if ( !IsPaused || !IsActivated.Value )
                return;

            IsPaused = false;

            if ( AiLocked && _lockedStateStored == false )
                Unlock();
        }

        private void InitializeFSM()
        {
            _enemyAIStateMachine.AddState( _stateFactory.CreateState<EnemyIdleState>() );
            _enemyAIStateMachine.AddState( _stateFactory.CreateState<EnemyChaseState>() );
            _enemyAIStateMachine.AddState( _stateFactory.CreateState<EnemyMeleeAttackState>() );
            _enemyAIStateMachine.AddState( _stateFactory.CreateState<EnemyRangeAttackState>() );
            _enemyAIStateMachine.AddState( _stateFactory.CreateState<EnemyDeadState>() );
            
            _enemyAIStateMachine.SetState<EnemyIdleState>();
        }

        private void CleanUpFSM()
        {
            _enemyAIStateMachine.CleanUp();
        }

        private void UpdateAnimator()
        {
        }
    }
}