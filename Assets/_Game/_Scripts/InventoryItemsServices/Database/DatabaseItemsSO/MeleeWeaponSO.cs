namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItemsSO
{
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using UnityEngine;

	[CreateAssetMenu( fileName = "MeleeWeapon", menuName = "Configs/Items/Database/MeleeWeapon" )]
	public class MeleeWeaponSO : WeaponSO
	{
		public override DatabaseItem GetItem()
		{
			return new MeleeWeapon( ID, Name, BaseWeaponAttributes, Icon );
		}
	}
}