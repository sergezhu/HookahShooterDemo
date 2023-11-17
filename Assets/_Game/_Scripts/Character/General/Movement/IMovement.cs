using UnityEngine;

namespace _Game._Scripts.Character.General.Movement
{
    public interface IMovement
    {
        public float MoveSpeed { get; }
        public float RotationSpeed { get; }
        public float CurrentSpeed { get; }
        public Vector3 Velocity { get; }
        public bool IsMoving { get; }
        public bool UseGravity { get; set; }

        public void MoveAlongDir(Vector2 input, bool rotate = true);
        public void MoveAlongDir(Vector3 worldDirection, bool rotate = true);
        public void Rotate(Vector2 input);
        public void Rotate(Vector3 direction);
        public void Rotate(Vector3 direction, float speed);
    }
}