namespace _Game._Scripts.InventoryItemsServices.Database
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using _Game._Scripts.Enums;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItemsSO;
	using _Game._Scripts.InventoryItemsServices.Static;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities;
	using UnityEngine;

	[Serializable]
	public struct ItemFactoryInfo
	{
		public InventoryItem Item;
	}
	
	
	[CreateAssetMenu( fileName = "ItemsLibrary", menuName = "Configs/Items/ItemsLibrary" )]
	public class ItemsLibrary : SerializedScriptableObject
	{
		[ListDrawerSettings(ShowPaging = false)]
		[SerializeField] private List<DatabaseItemSO> _databaseItems;

		private static readonly IEnumerable<(string, EEquipID)> CategoryPairs = new List<(string, EEquipID)>()
		{
			//(ItemsCategories.HeadArmorCategory, EEquipID.ArmorHead),
			//(ItemsCategories.BreastArmorCategory, EEquipID.ArmorBreast),
			//(ItemsCategories.LegsArmorCategory, EEquipID.ArmorLegs),
			//(ItemsCategories.FeetArmorCategory, EEquipID.ArmorFeet),
			(ItemsCategories.MeleeWeaponCategory, EEquipID.Weapon),
			(ItemsCategories.RangeWeaponCategory, EEquipID.Weapon),
			//(ItemsCategories.ConsumableCategory, EEquipID.Consumable),
			//(ItemsCategories.BagCategory, EEquipID.Bag),
		};

		private DatabaseItem TryGetItemByID( uint id )
		{
			DatabaseItem dbItem = null;
			
			var index = _databaseItems.FindIndex( def => def.ID == id );
			var isFounded = index != -1;

			if ( isFounded )
			{
				dbItem = _databaseItems[index].GetItem();
			}

			return dbItem;
		}

		private DatabaseItem TryGetItemByName( string name )
		{
			DatabaseItem dbItem = null;

			var index = _databaseItems.FindIndex( def => string.Equals( def.Name, name ) );
			var isFounded = index != -1;

			if ( isFounded )
			{
				dbItem = _databaseItems[index].GetItem();
			}

			return dbItem;
		}

		public void ValidateStack( ref InventoryItemsStack stack )
		{
			InventoryItemsStack restoredStack;
			
			if ( stack.ItemID == 0 )
			{
				restoredStack = RestoreID_2( stack );

				if ( restoredStack.ItemID > 0 )
					stack = restoredStack;
				else
					Debug.LogWarning( $"item {stack.ItemName} not found" );
			}
			else
			{
				restoredStack = RestoreName_2( stack );

				if ( string.IsNullOrEmpty( restoredStack.ItemName ) == false )
					stack = restoredStack; 
			}

			var stackSize = GetStackSize( stack.ItemID );

			var validatedCount = stackSize != -1
				? Mathf.Max( 1, Mathf.Min( stackSize, stack.ItemsCount ) )
				: stack.ItemsCount;

			stack = new InventoryItemsStack( stack.Item, validatedCount );
		}

		public void ValidateItem( ref InventoryItem item )
		{
			var stack = new InventoryItemsStack( item, 1 );
			
			ValidateStack( ref stack );

			item = stack.Item;
		}

		public uint FindItemID( string itemName )
		{
			DatabaseItem item = TryGetItemByName( itemName );

			if ( item == null )
			{
				Debug.LogWarning( $"<color=red>Item [{itemName}] not found in database</color>" );
				return 0;
			}
			
			return item.ID;
		}
		
		public uint FindItemID_2( string itemName )
		{
			return FindItemID( itemName );
		}

		public string FindItemName_2( uint itemId )
		{
			var item = TryGetItemByID( itemId );

			if ( item == null )
			{
				return string.Empty;
			}

			return item.Name;
		}

		public ItemFactoryInfo GetFactoryInfo( InventoryItem item )
		{
			var index = _databaseItems.FindIndex( d => d.ID == item.ID );
			var dbItem = _databaseItems[index];
			
			var info = new ItemFactoryInfo()
			{
				Item = item,
			};
			
			return info;
		}

		public InventoryItemsStack RestoreID( InventoryItemsStack stack )
		{
			return new InventoryItemsStack( new InventoryItem( FindItemID( stack.ItemName ), stack.ItemName ), Mathf.Max( 1, stack.ItemsCount ) );
		}

		public InventoryItemsStack RestoreID_2( InventoryItemsStack stack )
		{
			return new InventoryItemsStack( new InventoryItem( FindItemID_2( stack.ItemName ), stack.ItemName ), Mathf.Max(1, stack.ItemsCount) );
		}

		public InventoryItemsStack RestoreName_2( InventoryItemsStack stack )
		{
			return new InventoryItemsStack( new InventoryItem( stack.ItemID, FindItemName_2( stack.ItemID ) ), Mathf.Max( 1, stack.ItemsCount ) );
		}

		public Weapon GetWeapon( uint id )
		{
			DatabaseItem item = TryGetItemByID( id );

			bool isMelee = item is MeleeWeapon;
			bool isRange = item is RangeWeapon;

			if ( isMelee )
				return (MeleeWeapon)item;

			if ( isRange )
				return (RangeWeapon)item;

			//Debug.LogWarning( $"Item with id {id} is not weapon" );
			return null;
		}

		public bool IsItemInCategory( uint id, string category )
		{
			if ( id == 0 )
				return false;
			
			var item = TryGetItemByID( id );

			if ( item == null )
				return false; 
			
			bool isInCategory = string.Equals( GetItemCategory(item), category );

			return isInCategory;
		}

		public IEnumerable<InventoryItem> GetAllItems()
		{
			return _databaseItems
				.Select( item => new InventoryItem( item.ID, item.Name ) );
		}

		public IEnumerable<InventoryItem> GetAllItemsInCategory( string category )
		{
			if ( string.IsNullOrWhiteSpace( category.Trim() ) )
				throw new InvalidOperationException("Invalid category");

			return _databaseItems
				.Where( itemSO => string.Equals( GetItemCategory( itemSO.GetItem() ), category ) )
				.Select( itemSO => new InventoryItem(itemSO.ID, itemSO.Name) );
		}

		public IEnumerable<InventoryItem> GetAllItemsInCategories( string[] categories )
		{
			var items = new List<InventoryItem>();

			categories.ForEach( cat =>
			{
				var itemsInCategory = GetAllItemsInCategory( cat );
				items.AddRange( itemsInCategory );
			} );

			return items;
		}

		private string GetItemCategory( DatabaseItem item )
		{
			if ( item is MeleeWeapon )
				return ItemsCategories.MeleeWeaponCategory;

			if ( item is RangeWeapon )
				return ItemsCategories.RangeWeaponCategory;
			
			throw new InvalidOperationException();
		}

		public int GetStackSize( uint id )
		{
			if ( id == 0 )
				return -1;

			var item = TryGetItemByID( id );

			//return item.StackSize;
			return 1;
		}

		public DatabaseItem GetItem( uint id )
		{
			return TryGetItemByID( id );
		}
	}
}