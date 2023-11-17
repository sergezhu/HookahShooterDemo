namespace _Game._Scripts.InventoryItemsServices
{
	using System.Linq;
	using _Game._Scripts.InventoryItemsServices.Database;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItemsSO;
	using _Game._Scripts.Utilities.EditorHelpers;

	public static class DatabaseUtilityEditor
	{
		private const string ItemsLibraryPath = @"Assets/_Game/_Configs/ItemsLibrary.asset";
		private const string DatabaseWeaponsFolderPath = @"Assets/_Game/_Configs/ItemsLibrary.asset";
		
		private static ItemsLibrary _itemsLibrary;


		public static ItemsLibrary GetItemsLibrary()
		{
			#if UNITY_EDITOR
			var path = ItemsLibraryPath;

			if ( _itemsLibrary == null )
			{
				_itemsLibrary = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemsLibrary>( path );
			}
			#endif

			return _itemsLibrary;
		}

		#if UNITY_EDITOR
		public static bool IsIDNotUnique(uint id)
		{
			var allDatabaseItemSOs = EditorResourcesProvider.GetAllSOAssets<DatabaseItemSO>();
			var maxID = allDatabaseItemSOs.Count( dbItem => dbItem != null && dbItem.ID == id );

			return maxID > 1;
		}

		public static uint GetUniqueID()
		{
			var allDatabaseItemSOs = EditorResourcesProvider.GetAllSOAssets<DatabaseItemSO>();
			var maxID = allDatabaseItemSOs.Select( dbItem => dbItem == null ? 0 : dbItem.ID ).Max();

			return maxID + 1;
		}
		#endif
	}
}