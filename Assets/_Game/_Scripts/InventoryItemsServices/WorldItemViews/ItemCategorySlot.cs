namespace _Game._Scripts.InventoryItemsServices.WorldItemViews
{
	using System;
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Enums;
	using UniRx;

	public class ItemCategorySlot
	{
		private readonly ItemsCategorySlotView _view;
		private readonly UnitEquip _heroEquip;
		private readonly MeshItemView[] _itemsViews;
		private readonly CompositeDisposable _disposable;

		public ItemCategorySlot(ItemsCategorySlotView view, UnitEquip heroEquip)
		{
			_view = view;
			_heroEquip = heroEquip;

			_disposable = new CompositeDisposable();
		}

		public void Initialize()
		{
			UpdateViews();
			Subscribe();
		}

		public void CleanUp()
		{
			Unsubscribe();
		}

		private void Subscribe()
		{
			_heroEquip.CurrentWeapon
				.Subscribe( _ => OnCurrentWeaponChanged() )
				.AddTo( _disposable );
		}

		private void Unsubscribe()
		{
			_disposable.Clear();
		}

		private void OnCurrentWeaponChanged()
		{
			UpdateViews();
		}

		private void UpdateViews()
		{
			string equipName = string.Empty;

			switch ( _view.EquipViewID )
			{
				case EEquipViewID.ActiveWeapon:
					equipName = _heroEquip.CurrentWeapon.Value != null ? _heroEquip.CurrentWeapon.Value.Name : string.Empty;
					_view.SetItemViewByName( equipName );
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}