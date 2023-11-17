namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItems
{
	public class DatabaseItemStack<T> where T : DatabaseItem
	{
		public T Item { get; }
		public int ItemsCount { get; }

		public DatabaseItemStack( T item, int itemsCount )
		{
			Item = item;
			ItemsCount = itemsCount;
		}

		public static implicit operator InventoryItemsStack( DatabaseItemStack<T> dbStack )
		{
			var item = new InventoryItem( dbStack.Item.ID, dbStack.Item.Name );
			return new InventoryItemsStack(item, dbStack.ItemsCount);
		}
	}

	public class DatabaseItemStack
	{
		public DatabaseItem Item { get; }
		public int ItemsCount { get; }

		public DatabaseItemStack( DatabaseItem item, int itemsCount )
		{
			Item = item;
			ItemsCount = itemsCount;
		}

		public static implicit operator InventoryItemsStack( DatabaseItemStack dbStack )
		{
			var item = new InventoryItem( dbStack.Item.ID, dbStack.Item.Name );
			return new InventoryItemsStack( item, dbStack.ItemsCount );
		}
	}
}