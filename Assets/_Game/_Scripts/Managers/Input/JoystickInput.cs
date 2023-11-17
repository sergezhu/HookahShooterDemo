namespace _Game._Scripts.Managers.Input
{
    using UnityEngine;
    using Zenject;

    public class JoystickInput
    {
        private const float Epsilon = 0.01f;
        
        [Inject(Id = "Left")] private Joystick _leftZoneJoystick;
        [Inject(Id = "Right")] private Joystick _rightZoneJoystick;

        public bool IsTouchLeft => Mathf.Abs( _leftZoneJoystick.Vertical ) >= Epsilon || Mathf.Abs( _leftZoneJoystick.Horizontal ) >= Epsilon;
        public bool IsTouchRight => Mathf.Abs( _rightZoneJoystick.Vertical ) >= Epsilon || Mathf.Abs( _rightZoneJoystick.Horizontal ) >= Epsilon;
        public Vector2 ScreenDirectionLeft => _leftZoneJoystick.Direction;
        public Vector2 ScreenDirectionRight => _rightZoneJoystick.Direction;
    }
}