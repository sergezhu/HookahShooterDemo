namespace _Game._Scripts.InventoryItemsServices.WorldItemViews
{
	using System;
	using System.Collections.Generic;
	using _Game._Scripts.Character.Equip;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class ItemViewsController : MonoBehaviour, IInitializable, IDisposable
	{
		[SerializeField] private ItemsCategorySlotView[] _categorySlotsViews;

		private List<ItemCategorySlot> _categorySlots;
		private UnitEquip _heroEquip;
		
		private CompositeDisposable _disposable;

		[Inject]
		private void Construct( UnitEquip heroEquip )
		{
			_heroEquip = heroEquip;
			_disposable = new CompositeDisposable();
		}
		
		public void Initialize()
		{
			_categorySlotsViews.ForEach( view => view.Initialize() );

			_categorySlots = new List<ItemCategorySlot>();
			_categorySlotsViews.ForEach( view =>
			{
				var slot = new ItemCategorySlot( view, _heroEquip );
				slot.Initialize();
				_categorySlots.Add( slot );
			} );
		}

		public void CleanUp()
		{
			_categorySlots.ForEach( slot => slot.CleanUp() );
			_categorySlots.Clear();
			
			_disposable.Clear();
		}

		

		[Button]
		private void FindCategorySlots()
		{
			_categorySlotsViews = GetComponentsInChildren<ItemsCategorySlotView>();
		}

		public void Dispose()
		{
			CleanUp();
		}
	}
}