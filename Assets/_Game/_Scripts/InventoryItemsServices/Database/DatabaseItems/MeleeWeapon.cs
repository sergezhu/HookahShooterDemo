namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItems
{
	using System;
	using _Game._Scripts.InventoryItemsServices.Database.AttributesData;
	using UnityEngine;

	[Serializable]
	public class MeleeWeapon : Weapon
	{
		public MeleeWeapon( uint id, string name, BaseWeaponAttributes baseWeaponAttributes, Sprite icon ) : base( id, name, baseWeaponAttributes, icon )
		{
		}
	}
}