namespace _Game._Scripts.InventoryItemsServices
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using _Game._Scripts.InventoryItemsServices.Data;
	using _Game._Scripts.InventoryItemsServices.Database;
	using _Game._Scripts.InventoryItemsServices.Recipes;
	using _Game._Scripts.InventoryItemsServices.Static;
	using UnityEngine;

	public class InventoryUtility
	{
		public static IEnumerable<string> GetDefinitionsNames( string[] categories = null )
		{
			ItemsLibrary itemsLibrary = null;
			
			#if UNITY_EDITOR
			itemsLibrary = InventoryDatabaseEditModeProvider.GetItemsLibraryEditMode();
			#endif

			if ( itemsLibrary == null )
				throw new InvalidOperationException( "GetDefinitionsNames : itemsLibrary is null" );

			List<string> names;

			if ( categories == null )
			{
				var items = itemsLibrary.GetAllItems();
				names = items.Select( item => item.Name ).ToList();
			}
			else
			{
				var items = itemsLibrary.GetAllItemsInCategories( categories );
				names =items.Select( item => item.Name ).ToList();
			}

			return names;
		}
		
		public static string[] GetMaterialSkinnedCategories()
		{
			return new string[]
			{
				ItemsCategories.HeadArmorCategory,
				ItemsCategories.BreastArmorCategory,
				ItemsCategories.LegsArmorCategory,
				ItemsCategories.FeetArmorCategory
			};
		}

		public static IEnumerable<string> GetItemsRecipesCategoriesNames()
		{
			return new List<string>()
			{
				RecipesCategories.WoodWorkingCategory,
				RecipesCategories.StoneWorkingCategory,
				RecipesCategories.SmelterCategory,
				RecipesCategories.ArmorCraftCategory,
				RecipesCategories.WeaponCraftCategory,
				RecipesCategories.ConsumableCraftCategory,
				RecipesCategories.BulletsCraftCategory,
				RecipesCategories.ResourcesCraftCategory,
			};
		}

		public static IEnumerable<string> GetBuildingUpgradeRecipesCategoriesNames()
		{
			return new List<string>()
			{
				RecipesCategories.BuildingUpgradeCategory,
			};
		}

		public static bool CanInsertToStacks(InventoryItemsStack insertedStack, List<InventoryItemsStack?> stacks, int stackSize)
		{
			int insertedCount = insertedStack.ItemsCount;
			uint insertedItemID = insertedStack.ItemID;
			
			foreach ( var stack in stacks )
			{
				
				if ( stack == null )
				{
					insertedCount = Mathf.Max( 0, insertedCount - stackSize );

					if ( insertedCount == 0 )
						return true;
				}
				else if ( stack.Value.ItemID == insertedItemID )
				{
					var delta = stackSize - stack.Value.ItemsCount;
					insertedCount = Mathf.Max( 0, insertedCount - delta );

					if ( insertedCount == 0 )
						return true;
				}
			}

			return insertedCount == 0;
		}

		public static void InsertItemsToList( InventoryItem insertedItem, ref int remainedCount, ref List<InventoryItemsStack?> stacks, int stackSize, bool allowExpand,
											  Func<InventoryItemsStack, InventoryItemsStack> restoreFunc)
		{
			//remainedCount = insertedStack.ItemsCount;
			
			for ( var index = 0; index < stacks.Count; index++ )
			{
				var stack = stacks[index];

				if ( stack == null )
				{
					var iterationInsertedCount = Math.Min( stackSize, remainedCount );
					var iterationInsertedStack = new InventoryItemsStack( new InventoryItem( insertedItem.ID, "" ), iterationInsertedCount );

					if ( restoreFunc != null )
						iterationInsertedStack = restoreFunc.Invoke( iterationInsertedStack );

					stacks[index] = iterationInsertedStack;
					remainedCount -= iterationInsertedCount;
				}
				else
				{
					if ( stack.Value.ItemID == insertedItem.ID )
					{
						var iterationInsertedCount = Math.Min( stackSize - stack.Value.ItemsCount, remainedCount );
						var totalInsertedCount = stack.Value.ItemsCount + iterationInsertedCount;
						var iterationInsertedStack = new InventoryItemsStack( new InventoryItem( insertedItem.ID, "" ), totalInsertedCount );

						if ( restoreFunc != null )
							iterationInsertedStack = restoreFunc.Invoke( iterationInsertedStack );

						stacks[index] = iterationInsertedStack;
						remainedCount -= iterationInsertedCount;
					}
				}

				if ( remainedCount == 0 )
					break;
			}

			if ( remainedCount > 0 && allowExpand )
			{
				var expandedStack = new InventoryItemsStack( new InventoryItem( insertedItem.ID, "" ), remainedCount );
				stacks.Add( expandedStack );
			}
		}

		public static void RemoveStackFromList( InventoryItem removedItem, ref int requiredToRemoveCount, ref List<InventoryItemsStack?> stacks )
		{
			for ( var index = 0; index < stacks.Count; index++ )
			{
				var stack = stacks[index];

				if ( stack == null || stack.Value.ItemID != removedItem.ID )
					continue;

				var iterationRequiredToRemoveCount = Math.Min( requiredToRemoveCount, stack.Value.ItemsCount );
				var afterRemoveStackCount = stack.Value.ItemsCount - iterationRequiredToRemoveCount;

				if ( afterRemoveStackCount == 0 )
				{
					stacks[index] = null;
				}
				else
				{
					var afterRemoveStack = new InventoryItemsStack( new InventoryItem( removedItem.ID, removedItem.Name ), afterRemoveStackCount );
					stacks[index] = afterRemoveStack;
				}

				requiredToRemoveCount -= iterationRequiredToRemoveCount;

				if ( requiredToRemoveCount == 0 )
					break;
			}
		}

		public static void AddInfoAboutHavingItemsCount( IEnumerable<CraftSourceIngredientCellData> sourceIngredientsData,
														 out CraftSourceIngredientCellData[] outputIngredientsData, List<InventoryItemsStack?> pocketsData, 
														 List<InventoryItemsStack?> firstBagData, List<InventoryItemsStack?> mainStorageData, bool includeStorage  )
		{
			outputIngredientsData = sourceIngredientsData.ToArray();

			for ( var index = 0; index < outputIngredientsData.Length; index++ )
			{
				var ingredient = outputIngredientsData[index];

				var inPocketsCount = pocketsData
					.Where( s => s != null && s.Value.ItemID == ingredient.Item.ID )
					.Sum( s => s.Value.ItemsCount );

				var inBag1Count = firstBagData
					.Where( s => s != null && s.Value.ItemID == ingredient.Item.ID )
					.Sum( s => s.Value.ItemsCount );

				var inStorageCount = 0;

				if ( includeStorage )
				{
					inStorageCount = mainStorageData
						.Where( s => s != null && s.Value.ItemID == ingredient.Item.ID )
						.Sum( s => s.Value.ItemsCount );
				}

				var totalCount = inPocketsCount + inBag1Count + inStorageCount;
				outputIngredientsData[index] = new CraftSourceIngredientCellData( ingredient, totalCount );
			}
		}
	}
}