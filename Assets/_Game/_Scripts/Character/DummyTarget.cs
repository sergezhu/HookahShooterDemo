namespace _Game._Scripts.Character
{
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Weapons;
	using UnityEngine;

	public class DummyTarget : MonoBehaviour, IDamageableTarget
	{
		public void TakeDamage( Damage damage )
		{
		}

		public Vector3 Position => transform.position;
		public Quaternion Rotation => transform.rotation;
		public Vector3 AimTargetPosition => Position;
		public Vector3 Forward => transform.forward;
		public string Name => name;
		public bool IsEnabled => gameObject.activeSelf;
		public int SiblingIndex => transform.GetSiblingIndex();
		public Vector3 GetClosestPoint( Vector3 fromPos )
		{
			return Position;
		}
	}
}