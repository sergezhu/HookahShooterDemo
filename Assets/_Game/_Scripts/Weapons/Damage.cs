namespace _Game._Scripts.Weapons
{
	using _Game._Scripts.Enums;
	using UnityEngine;

	[System.Serializable]
	public struct Damage
	{
		public float Value;
		public Vector3 HitForce;
		public Vector3 Position;
		
		public EWeaponDistanceType DistanceType { get; }

		public Damage( EWeaponDistanceType distanceType ) : this()
		{
			DistanceType = distanceType;
		}
	}
}