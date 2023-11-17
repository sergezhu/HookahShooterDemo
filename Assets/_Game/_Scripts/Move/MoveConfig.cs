namespace _Game._Scripts.Move
{
	using UnityEngine;

	[CreateAssetMenu( fileName = "MoveConfig", menuName = "Configs/MoveConfig" )]
	public class MoveConfig : ScriptableObject
	{
		public float MoveSpeed = 4f;
		public float MeshAgentSpeed = 5f;
		public float NavmeshSampleDistance = 0.05f;
		public float RotateSpeed = 300;
		public float Acceleration;
		public float Deceleration;
		public float Friction;
		[Range( 0, 1 )]
		public float DampAngularVelocityMul = 1;

		[Header( "Rotation" )]
		public float RotationSmoothTimeSlow;
		public float RotationSmoothTimeFast;
	}
}

