namespace _Game._Scripts.InventoryItemsServices.Data
{
	using System;
	using _Game._Scripts.InventoryItemsServices.Database;
	using UnityEngine;

	[Serializable]
	public struct CraftSourceIngredientCellData
	{
		[SerializeField] private InventoryItem _item;
		[SerializeField] private Sprite _icon;
		[SerializeField] private Sprite _categoryIcon;
		[SerializeField] private int _requiredCount;
		[SerializeField] private int _totalCount;

		public CraftSourceIngredientCellData( InventoryItem item, Sprite icon, Sprite categoryIcon, int requiredCount, int totalCount )
		{
			_item = item;
			_icon = icon;
			_categoryIcon = categoryIcon;
			_requiredCount = requiredCount;
			_totalCount = totalCount;
		}

		public CraftSourceIngredientCellData( CraftSourceIngredientCellData data, int totalCount )
		{
			_item = data.Item;
			_icon = data.Icon;
			_categoryIcon = data.CategoryIcon;
			_requiredCount = data.RequiredCount;
			_totalCount = totalCount;
		}

		public CraftSourceIngredientCellData( InventoryItem item, Sprite icon, Sprite categoryIcon, int requiredCount )
		{
			_item = item;
			_icon = icon;
			_categoryIcon = categoryIcon;
			_requiredCount = requiredCount;
			_totalCount = 0;
		}

		public int RequiredCount => _requiredCount;
		public int TotalCount => _totalCount;

		public InventoryItem Item => _item;

		public Sprite Icon => _icon;
		public Sprite CategoryIcon => _categoryIcon;

		public InventoryItemsStack AsStack()
		{
			return new InventoryItemsStack( _item, _requiredCount );
		}
	}
}