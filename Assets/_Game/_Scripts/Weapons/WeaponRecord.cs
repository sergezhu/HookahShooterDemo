namespace _Game._Scripts.Weapons
{
	using System;
	using System.Collections.Generic;
	using _Game._Scripts.InventoryItemsServices;
	using _Game._Scripts.InventoryItemsServices.Static;
	using Sirenix.OdinInspector;
	using UnityEngine;

	[Serializable]
	public struct WeaponRecord
	{
		[SerializeField, ReadOnly] private uint _weaponID;
		[ValueDropdown("GetDefinitionsNames")]
		[InlineButton( "ResetItem", "Reset" )]
		[SerializeField] private string _weaponName;

		public WeaponRecord( uint id, string name )
		{
			_weaponID = id;
			_weaponName = name;
		}

		public uint ID => _weaponID;
		public string Name => _weaponName;

		private IEnumerable<string> GetDefinitionsNames() => InventoryUtility.GetDefinitionsNames( new[]
		{
			ItemsCategories.MeleeWeaponCategory, 
			ItemsCategories.RangeWeaponCategory
		} );

		public void SetID( uint id ) => _weaponID = id;

		private void ResetItem()
		{
			_weaponID = 0;
			_weaponName = string.Empty;
		}
	}
}