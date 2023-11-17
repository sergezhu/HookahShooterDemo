namespace _Game._Scripts.Configs
{
	using Sirenix.OdinInspector;
	using UnityEngine;
	using Zenject;

	[CreateAssetMenu( fileName = "HeroConfig", menuName = "Configs/HeroConfig" )]
	public class HeroConfig : ScriptableObject, IInitializable
	{

		[Title("Live Settings")]
		[field: SerializeField, Space] public float MaxHealth { get; private set; }
		[field: SerializeField] public float RessurectHealth { get; private set; }
		
		[field: SerializeField] public float LiveParametersTweenDuration { get; private set; } 
		[field: SerializeField, Range(0, 1f)] public float DebugStartHealthModifier { get; private set; } 
		[field: SerializeField, Range( 0, 1f )] public float DebugStartEnergyModifier { get; private set; } 
		
		[Title("AI Settings")]
		[field: SerializeField, Space] public float FieldOfViewDistance { get; private set; }
		[field: SerializeField] public float DeltaAimDistance { get; private set; }
		[field: SerializeField] public bool IsAutoMeleeAttackEnabled { get; private set; }
		[field: SerializeField] public bool IsAutoRangeAttackEnabled { get; private set; }
		[field: SerializeField] public bool IsShootingWhenMovingEnabled { get; private set; }
		
		[Title("Animator")]
		[field: SerializeField] public float DisableAimDelay { get; private set; }


		public void Initialize()
		{
		}
	}
}