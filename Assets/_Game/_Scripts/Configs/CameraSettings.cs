namespace _Game._Scripts.Configs
{
	using UnityEngine;

	[CreateAssetMenu( fileName = "CameraSettings", menuName = "Configs/CameraSettings" )]
	public class CameraSettings : ScriptableObject
	{
		public float MinCameraTilt;
		public float MaxCameraTilt;
		public float RotationSensX;
		public float RotationSensY;

	}
}