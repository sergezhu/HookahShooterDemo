namespace _Game._Scripts.InventoryItemsServices.Database.AttributesData
{
	using System;
	using _Game._Scripts.Enums;

	[Serializable]
	public struct BaseWeaponAttributes
	{
		public float AttackPowerMin;
		public float AttackPowerMax;
		public float AttackDelay;
		public float AttackDistance;
		public float WeaponAnimationSpeed;
		public float HitForceModifier;
		public EWeaponAnimation WeaponAnimation;
	}
}