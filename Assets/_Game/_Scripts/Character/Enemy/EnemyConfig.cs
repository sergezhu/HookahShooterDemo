namespace _Game._Scripts.Character.Enemy
{
	using UnityEngine;

	public class EnemyConfig : MonoBehaviour
	{
		[field: SerializeField, Space] public float MaxHealth { get; private set; }
		[field: SerializeField] public float Armor { get; private set; }
		[field: SerializeField, Space] public float FieldOfViewDistance { get; private set; } 
		[field: SerializeField] public float AimDelay { get; private set; }

		[field: SerializeField, Space] public float DestroyAfterDeadDelay { get; private set; } 
		[field: SerializeField] public float DestroyAfterDeadDuration { get; private set; } 
		[field: SerializeField] public float DestroyOffsetY { get; private set; } 
	}
}