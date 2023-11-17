namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItemsSO
{
	using System;
	using _Game._Scripts.InventoryItemsServices.Database.AttributesData;
	using UnityEngine;

	[Serializable]
	public abstract class WeaponSO : DatabaseItemSO
	{
		[SerializeField] private BaseWeaponAttributes _baseWeaponAttributes;


		protected BaseWeaponAttributes BaseWeaponAttributes => _baseWeaponAttributes;
	}
}	