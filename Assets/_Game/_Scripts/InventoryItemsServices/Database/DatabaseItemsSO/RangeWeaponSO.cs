namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItemsSO
{
	using _Game._Scripts.InventoryItemsServices.Database.AttributesData;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using UnityEngine;

	[CreateAssetMenu( fileName = "RangeWeapon", menuName = "Configs/Items/Database/RangeWeapon" )]
	public class RangeWeaponSO : WeaponSO
	{
		[SerializeField] private RangeWeaponAttributes _rangeWeaponAttributes;

		protected RangeWeaponAttributes RangeWeaponAttributes => _rangeWeaponAttributes;
		
		public override DatabaseItem GetItem()
		{
			return new RangeWeapon( ID, Name, BaseWeaponAttributes, RangeWeaponAttributes, Icon );
		}
	}
}