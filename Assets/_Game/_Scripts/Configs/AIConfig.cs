namespace _Game._Scripts.Configs
{
	using UnityEngine;

	[CreateAssetMenu( fileName = "AIConfig", menuName = "Configs/AIConfig" )]
	public class AIConfig : ScriptableObject
	{
		public float SqrAchieveTargetMinDistance = 0.01f;
		public float MoveToTargetPreparingDuration = 1.5f;
		public float ChaseDurationWhenTargetOutOfSight = 2.5f;
		public float AggroDurationWhenDamaged = 5f;
	}
}