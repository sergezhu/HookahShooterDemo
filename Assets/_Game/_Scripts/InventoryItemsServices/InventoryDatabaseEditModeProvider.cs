namespace _Game._Scripts.InventoryItemsServices
{
	#if UNITY_EDITOR
	using _Game._Scripts.InventoryItemsServices.Database;
	using UnityEditor;

	public class InventoryDatabaseEditModeProvider
	{
		private static ItemsLibrary _itemsLibrary;

		public static ItemsLibrary GetItemsLibraryEditMode()
		{
			var path = "Assets/_Game/_Configs/ItemsLibrary.asset";
			_itemsLibrary = AssetDatabase.LoadAssetAtPath<ItemsLibrary>( path );

			return _itemsLibrary;
		}
	}
	
	#endif
}