namespace _Game._Scripts.InventoryItemsServices.Database
{
	using System;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;
	using UnityEngine;

	[Serializable]
	public struct InventoryItem
	{
		[HorizontalGroup( "G1", LabelWidth = 20, Width = 150, MarginRight = 20 )]
		[SerializeField, ReadOnly] private uint _id;
		[ValueDropdown("GetDefinitionsNames")]
		[HorizontalGroup( "G1", LabelWidth = 50 ), InlineButton( "Reset" )]
		[SerializeField] private string _name;

		public InventoryItem( uint id, string name )
		{
			_id = id;
			_name = name;
		}

		public InventoryItem( string name )
		{
			_id = 0;
			_name = name;
		}

		public uint ID => _id;
		public string Name => _name;

		private IEnumerable<string> GetDefinitionsNames() => InventoryUtility.GetDefinitionsNames();

		private void Reset()
		{
			_id = 0;
			_name = string.Empty;
		}
	}
}