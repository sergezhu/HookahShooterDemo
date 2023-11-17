namespace _Game._Scripts.Character.Equip
{
	using _Game._Scripts.Enums;
	using _Game._Scripts.InventoryItemsServices.Database;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.Weapons;
	using Sirenix.OdinInspector;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UnitEquip : MonoBehaviour, IInitializable
	{
		[SerializeField, HideLabel] private WeaponRecord _weapon;

		private ItemsLibrary _itemsLibrary;

		public ReactiveProperty<Weapon> CurrentWeapon { get; } = new ReactiveProperty<Weapon>();

		public bool IsActiveWeaponMelee => CurrentWeapon.Value is MeleeWeapon;
		public bool IsActiveWeaponRange => CurrentWeapon.Value is RangeWeapon;
		public bool HasWeapon => CurrentWeapon.Value != null;


		public float WeaponAttackDistance => CurrentWeapon.Value?.AttackDistance ?? 0;
		public float WeaponAttackDelay => CurrentWeapon.Value.AttackDelay;
		public float WeaponAttackPowerMin => CurrentWeapon.Value.AttackPowerMin;
		public float WeaponAttackPowerMax => CurrentWeapon.Value.AttackPowerMax;
		public float WeaponAnimationSpeed => CurrentWeapon.Value?.WeaponAnimationSpeed ?? 1;
		public EWeaponAnimation WeaponAnimation => CurrentWeapon.Value?.WeaponAnimation ?? EWeaponAnimation.None;
		public float WeaponRotateCompensationY => IsActiveWeaponRange ? ((RangeWeapon)CurrentWeapon.Value).OwnerRotateCompensationY : 0;
		

		[Inject]
		public void Construct( ItemsLibrary itemsLibrary )
		{
			_itemsLibrary = itemsLibrary;
		}

		public void Initialize()
		{
			CurrentWeapon.Value = _itemsLibrary.GetWeapon( _weapon.ID );
		}


		private void OnValidate()
		{
			#if UNITY_EDITOR

			bool isPrefabAsset = UnityEditor.PrefabUtility.IsPartOfPrefabAsset( gameObject );
			bool isPrefabInstance = UnityEditor.PrefabUtility.IsPartOfPrefabInstance( gameObject );

			if ( isPrefabAsset && !isPrefabInstance )
			{
				Debug.Log( $"[{gameObject.name}] is a prefab asset and not instantiated." );
				return;
			}
			
			var path = "Assets/_Game/_Configs/ItemsLibrary.asset";
			_itemsLibrary = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemsLibrary>( path );
			
			#endif

			if ( _itemsLibrary == null )
				return;

			ValidateItem();
		}

		private void ValidateItem()
		{
			var item = new InventoryItem(_weapon.ID, _weapon.Name);
			_itemsLibrary.ValidateItem( ref item );
			_weapon = new WeaponRecord( item.ID, item.Name );
		}

		public void CleanUp()
		{
			CurrentWeapon?.Dispose();
		}
	}
}