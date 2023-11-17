namespace _Game._Scripts.Configs
{
	using UnityEngine;

	[CreateAssetMenu( fileName = "BattleConfig", menuName = "Configs/BattleConfig" )]
	public class BattleConfig : ScriptableObject
	{
		public float BaseMeleeDeadHitRagdollForce;
		public float BaseRangeDeadHitRagdollForce;

		[Space]
		public float BaseExplosionHitRagdollForce;
		public AnimationCurve ExplosionForceFactorFromRadius;
		public AnimationCurve ExplosionDamageFactorFromRadius;
	}
}