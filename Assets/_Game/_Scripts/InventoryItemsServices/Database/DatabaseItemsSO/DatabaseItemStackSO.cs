namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItemsSO
{
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;

	public class DatabaseItemStackSO<T> where T : DatabaseItemSO
	{
		public T Item { get; }
		public int ItemsCount { get; }

		public DatabaseItemStackSO( T item, int itemsCount )
		{
			Item = item;
			ItemsCount = itemsCount;
		}

		public static implicit operator InventoryItemsStack( DatabaseItemStackSO<T> dbStack )
		{
			var item = new InventoryItem( dbStack.Item.ID, dbStack.Item.Name );
			return new InventoryItemsStack(item, dbStack.ItemsCount);
		}
	}

	public class DatabaseItemStackSO
	{
		public DatabaseItem Item { get; }
		public int ItemsCount { get; }

		public DatabaseItemStackSO( DatabaseItem item, int itemsCount )
		{
			Item = item;
			ItemsCount = itemsCount;
		}

		public static implicit operator InventoryItemsStack( DatabaseItemStackSO dbStack )
		{
			var item = new InventoryItem( dbStack.Item.ID, dbStack.Item.Name );
			return new InventoryItemsStack( item, dbStack.ItemsCount );
		}
	}
}