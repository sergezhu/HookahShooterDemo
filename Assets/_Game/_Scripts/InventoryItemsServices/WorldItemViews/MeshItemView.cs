namespace _Game._Scripts.InventoryItemsServices.WorldItemViews
{
	using _Game._Scripts.InventoryItemsServices.Database;
	using UnityEngine;

	public class MeshItemView : MonoBehaviour
	{
		[SerializeField] private InventoryItem _databaseItem;

		public string ItemName => _databaseItem.Name;
	}
}