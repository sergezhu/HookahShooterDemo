using UnityEngine;
using Zenject;

namespace _Game._Scripts.Character.General.Movement
{
    using _Game._Scripts.Utilities.Extensions;

    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour, IMovement, IInitializable
    {
    #region Variables
        [Header("Speed")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 100f;

        [Header( "Navigation" )]
        [SerializeField] private float _samplePositionDistance = 0.5f;
        [SerializeField] private float _verticalOffset = 0f;

        [Header("Gravity")]
        [SerializeField] private float _gravity = -12f;
        [SerializeField] private float _groundedGravity = -2f;
        [SerializeField] private bool _useGravity = true;
    #endregion

        private Vector3 _currentPosition;
        private Vector3 _lastPosition;

    #region Properties
        public CharacterController CharacterController { get; private set; }
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float CurrentSpeed { get; private set; }
        public Vector3 CurrentVelocity { get; private set; }
        public Vector3 Velocity => CharacterController.velocity;
        public float SamplePositionDistance => _samplePositionDistance;

        public bool IsMoving => CurrentSpeed > 0.1f;

        public bool UseGravity
        {
            get => _useGravity; 
            set => _useGravity = value;
        }

    

    #endregion

        private void Update()
        {
            if ( CharacterController.enabled )
                TryApplyGravity();

            //CalculateSpeed();
        }

        public void Initialize()
        {
            CharacterController = GetComponent<CharacterController>();

            CharacterController.detectCollisions = true;
        
            _lastPosition = transform.position;
        }

        public void Activate()
        {
            enabled = true;
            CharacterController.enabled = true;
        }

        public void Deactivate()
        {
            enabled = false;
            CharacterController.enabled = false;
        }

        public Vector3 GetNextPoint( Vector3 position, Vector3 direction, float offset )
        {
            var nextPointPosition = position + direction.normalized * (offset + Time.deltaTime * MoveSpeed);
            return nextPointPosition;
        }

        public void MoveAlongDir(Vector2 input, bool rotate = true)
        {
            Vector3 inputDirection = new Vector3(input.x, 0, input.y);
            MoveAlongDir(inputDirection, rotate);
        }

        public void MoveAlongDir(Vector3 worldDirection, bool rotate = true)
        {
            var deltaPos = (worldDirection.normalized * Time.deltaTime * MoveSpeed).WithY( 0 );
        
            if ( CharacterController.enabled )
                CharacterController.Move( deltaPos );
            else
                transform.position += deltaPos + _verticalOffset * Vector3.up;
        
            if (rotate) 
                Rotate(worldDirection);

            CalculateSpeed( deltaPos );
            CalculateVelocity( deltaPos );
        }
        public void MoveTo(Vector3 targetPosition, bool rotate = true)
        {
            var deltaPos = (targetPosition + _verticalOffset * Vector3.up - transform.position);

            if ( CharacterController.enabled )
            {
                CharacterController.Move( deltaPos );
            }
            else
            {
                transform.position += deltaPos;
            }
        
            if (rotate) 
                Rotate(deltaPos);

            CalculateSpeed( deltaPos );
            CalculateVelocity( deltaPos );
        }

        public void StopMove()
        {
            //Debug.Log( $"<color=red>StopMove</color>" );
            CurrentSpeed = 0;
            CurrentVelocity = Vector3.zero;
        }

        public void Rotate(Vector2 input)
        {
            Vector3 direction = new Vector3(input.x, 0, input.y);
            Rotate(direction);
        }

        public void Rotate(Vector3 direction)
        {
            Rotate(direction, RotationSpeed);
        }

        public void Rotate(Vector3 direction, float speed)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            angle = angle <= 180 ? angle : angle - 360;

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(currentRotation.x, angle, currentRotation.z);

            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * speed);
        }

        private void TryApplyGravity()
        {
            if (UseGravity == false) 
                return;

            var gravitySpeed = CharacterController.isGrounded ? _groundedGravity : _gravity;
            CharacterController.Move(new Vector3(0, gravitySpeed, 0) * Time.deltaTime);
        }

        private void CalculateSpeed()
        {
            _currentPosition = transform.position;

            CurrentSpeed = Mathf.Approximately( Time.deltaTime, 0 ) == false
                ? (_currentPosition - _lastPosition).magnitude / Time.deltaTime
                : 0;
        
            _lastPosition = _currentPosition;
        }
    
        private void CalculateSpeed(Vector3 deltaPos)
        {
            CurrentSpeed = Mathf.Approximately( Time.deltaTime, 0 ) == false
                ? deltaPos.magnitude / Time.deltaTime
                : 0;
        }

        private void CalculateVelocity( Vector3 deltaPos )
        {
            CurrentVelocity = Mathf.Approximately( Time.deltaTime, 0 ) == false
                ? deltaPos / Time.deltaTime
                : Vector3.zero;
        }
    }
}