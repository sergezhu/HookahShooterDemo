namespace _Game._Scripts.InventoryItemsServices.Database
{
	using System;
	using Sirenix.OdinInspector;
	using UnityEngine;

	[Serializable]
	public struct InventoryItemsStack
	{
		[HideLabel]
		//[InlineButton("ResetItem", "Reset")]
		[SerializeField] private InventoryItem _item;
		[SerializeField, Min(1)] private int _itemsCount;

		public InventoryItemsStack( InventoryItem item, int itemsCount )
		{
			_item = item;
			_itemsCount = itemsCount;
		}

		public InventoryItem Item => _item;
		public uint ItemID => _item.ID;
		public string ItemName => _item.Name;
		public int ItemsCount => _itemsCount;

		private void ResetItem()
		{
			_item = new InventoryItem();
		}
	}
}