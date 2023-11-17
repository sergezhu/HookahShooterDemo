namespace _Game._Scripts.InventoryItemsServices.WorldItemViews
{
	using _Game._Scripts.Enums;
	using _Game._Scripts.Utilities.Extensions;
	using Sirenix.Utilities;
	using UnityEngine;

	public class ItemsCategorySlotView : MonoBehaviour
	{
		private MeshItemView[] _worldItemsViews;
		[field: SerializeField] public EEquipViewID EquipViewID { get; private set; }

		public void Initialize()
		{
			_worldItemsViews = GetComponentsInChildren<MeshItemView>(true);
		}

		public void SetItemViewByName( string itemName )
		{
			//Debug.Log( $"SetItemViewByName for {EquipViewID} : {itemName}" );
			
			LinqExtensions.ForEach( _worldItemsViews, itemView =>
			{
				if ( string.Equals( itemName, itemView.ItemName ) )
					itemView.Show();
				else
					itemView.Hide();
			} );
		}
	}
}