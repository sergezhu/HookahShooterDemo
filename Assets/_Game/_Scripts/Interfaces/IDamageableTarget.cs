namespace _Game._Scripts.Interfaces
{
	using UnityEngine;

	public interface IDamageableTarget : IDamageable, ITarget
	{
		public Vector3 AimTargetPosition { get; }
	}
}