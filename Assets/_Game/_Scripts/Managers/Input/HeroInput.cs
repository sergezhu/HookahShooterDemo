namespace _Game._Scripts.Managers.Input
{
    using System;
    using _Game._Scripts.Level;
    using _Game._Scripts.UI.Hud;
    using _Game._Scripts.Utilities.Extensions;
    using DG.Tweening;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class HeroInput: IInitializable, ITickable
    {
        private readonly JoystickInput _joystickInput;
        private readonly KeyboardInput _keyboardInput;
        private readonly HudController _hudController;
        private readonly CMCameraController _cameraController;
        private readonly CompositeDisposable _disposable;
        
        private Vector2 _keyboardDirection;
        private Tween _attackRollbackTween;


        public bool LockMoveInput { get; set; }
        public Vector3 WorldDirection { get; private set; }
        public bool IsMoving => (IsLeftZoneJoystickMoving || IsKeyboardMoving) && LockMoveInput == false;

        public IObservable<bool> IsAttackButtonPressed { get; private set; }

        private bool IsLeftZoneJoystickMoving => _joystickInput.IsTouchLeft;
        //private bool IsRightZoneJoystickMoving => _joystickInput.IsTouchRight;
        private bool IsKeyboardMoving => _keyboardDirection.sqrMagnitude > 1e-6;

        public HeroInput(JoystickInput joystickInput, KeyboardInput keyboardInput, HudController hudController, CMCameraController cameraController)
        {
            _joystickInput = joystickInput;
            _keyboardInput = keyboardInput;
            _hudController = hudController;
            _cameraController = cameraController;

            _disposable = new CompositeDisposable();
        }

        public void Initialize()
        {
            IsAttackButtonPressed = _hudController.IsAttackButtonPressed;

            _keyboardInput.MoveDirection
                .Subscribe(dir =>
                {
                    //Debug.Log( $"kb dir : {dir}" );
                    _keyboardDirection = dir;
                } )
                .AddTo( _disposable );
        }

        public void Tick()
        {
            //var rightJoysticDirection = IsRightZoneJoystickMoving ? _joystickInput.ScreenDirectionRight : Vector2.zero;
            
            var camRotationY = _cameraController.RotationAngleY;
            var rotation = Quaternion.Euler( 0, camRotationY, 0 );

            var screenDir = IsLeftZoneJoystickMoving ? _joystickInput.ScreenDirectionLeft : _keyboardDirection;
            WorldDirection = rotation * screenDir.x0y();

            
        }
        
        public void SetIsEnemyInAttackRange(bool value)
        {
            _hudController.SetAttackButtonInRange( value );

            /*if(value)
                _hudController.EnableAttackButton();
            else
                _hudController.DisableAttackButton();*/
        }

        public void SetIsInteractableInRange( bool value )
        {
            /*if ( value )
                _hudController.EnableInteractButton();
            else
                _hudController.DisableInteractButton();*/
        }

        public void SetWeaponRollbackProgress( float value )
        {
            //_hudController.SetProgressAttackButton( 1f - value );
        }
    }
}