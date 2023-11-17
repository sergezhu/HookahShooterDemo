namespace _Game._Scripts.Weapons
{
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using UnityEngine;

	public struct ProjectileRequestData
	{
		public int ID;
		public RangeWeapon DatabaseWeapon;
		public float Damage;
		public float HitForceModifier;
		public IWeaponView WeaponView;
		public Vector3 AimPosition;
		public ProjectileOwner Owner;
	}
}