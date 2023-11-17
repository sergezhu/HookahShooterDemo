namespace _Game._Scripts.Managers.Input
{
    using _Game._Scripts.Configs;
    using _Game._Scripts.Level;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class CameraRotator: IInitializable, ITickable
    {
        private readonly JoystickInput _joystickInput;
        private readonly CMCameraController _cameraController;
        private readonly CameraSettings _cameraSettings;
        private readonly CompositeDisposable _disposable;

        private Vector3 _accumulatedRotationEuler;
        private Vector3 _defaultCameraRotation;
        private bool IsRightZoneJoystickMoving => _joystickInput.IsTouchRight;

        public CameraRotator(JoystickInput joystickInput, CMCameraController cameraController, CameraSettings cameraSettings)
        {
            _joystickInput = joystickInput;
            _cameraController = cameraController;
            _cameraSettings = cameraSettings;

            _disposable = new CompositeDisposable();
        }

        public void Initialize()
        {
            _defaultCameraRotation = _cameraController.GetHeroCameraRotation();
            _accumulatedRotationEuler = Vector3.zero;
        }

        public void Tick()
        {
           //Debug.Log( $"{_joystickInput.ScreenDirectionRight.x} : {_joystickInput.ScreenDirectionRight.y}" );
            
            var rightJoysticDirection = IsRightZoneJoystickMoving ? _joystickInput.ScreenDirectionRight : Vector2.zero;

            if ( IsRightZoneJoystickMoving )
            {
                var deltaRotationEuler = new Vector3( -1f * rightJoysticDirection.y * _cameraSettings.RotationSensY, rightJoysticDirection.x * _cameraSettings.RotationSensX, 0 );
                _accumulatedRotationEuler += deltaRotationEuler;

                var resultEuler = _defaultCameraRotation + _accumulatedRotationEuler;
                var clampAngleX = Mathf.Clamp( resultEuler.x, _cameraSettings.MinCameraTilt, _cameraSettings.MaxCameraTilt );
                resultEuler.x = clampAngleX;

                _accumulatedRotationEuler = resultEuler - _defaultCameraRotation;
                
                _cameraController.SetHeroCameraRotation( resultEuler );
            }
            else
            {
                //Debug.Log( "IsRightZoneJoystickMoving false" );
            }
        }
    }
}