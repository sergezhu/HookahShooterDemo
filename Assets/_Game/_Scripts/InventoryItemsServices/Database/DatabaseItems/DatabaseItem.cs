namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItems
{
	using System;
	using UnityEngine;

	[Serializable]
	public abstract class DatabaseItem
	{
		[SerializeField] private uint _id;
		[SerializeField] private string _name;
		[SerializeField] private Sprite _icon;


		public DatabaseItem( uint id, string name, Sprite icon )
		{
			_id = id;
			_name = name;
			_icon = icon;
		}


		public uint ID => _id;
		public string Name => _name;
		public Sprite Icon => _icon;

		public static implicit operator InventoryItem( DatabaseItem dbItem )
		{
			var item = new InventoryItem( dbItem._id, dbItem._name );
			return item;
		}

		public void ValidateID( uint id )
		{
			_id = id;
		}
	}
}