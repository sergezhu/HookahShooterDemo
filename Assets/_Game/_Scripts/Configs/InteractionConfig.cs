namespace _Game._Scripts.Configs
{
	using UnityEngine;

	[CreateAssetMenu( fileName = "InteractionConfig", menuName = "Configs/InteractionConfig" )]
	public class InteractionConfig : ScriptableObject
	{
		[SerializeField] private float _deadScreenDelay;
		

		public float DeadScreenDelay => _deadScreenDelay;
	}
}