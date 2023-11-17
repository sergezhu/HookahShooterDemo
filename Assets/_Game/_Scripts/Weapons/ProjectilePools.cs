namespace _Game._Scripts.Weapons
{
	using System.Collections.Generic;
	using System.Linq;
	using _Game._Scripts.InventoryItemsServices.Database;
	using _Game._Scripts.InventoryItemsServices.Static;
	using QFSW.MOP2;
	using UnityEngine;
	using UnityEngine.Serialization;

	[CreateAssetMenu( fileName = "ProjectilePools", menuName = "Configs/ProjectilePools" )]
	public class ProjectilePools : ScriptableObject
	{
		[FormerlySerializedAs( "_flyProjectilePools" )] 
		[SerializeField, Space] private List<WeaponProjectilePoolRecord> _projectilePools;
		
		private ItemsLibrary _itemsLibrary;

		public ObjectPool[] GetUniqueFlyPools()
		{
			return _projectilePools
				.Where( pp => pp.FlyFxPool != null )
				.Select( pp => pp.FlyFxPool )
				.Distinct()
				.ToArray(); 
		}

		public ObjectPool[] GetUniqueHitFxPools()
		{
			return _projectilePools
				.Where( pp => pp.HitFxPool != null )
				.Select( pp => pp.HitFxPool )
				.Distinct()
				.ToArray();
		}

		public ObjectPool[] GetUniqueHitDecalPools()
		{
			return _projectilePools
				.Where( pp => pp.HitDecalPool != null )
				.Select( pp => pp.HitDecalPool )
				.Distinct()
				.ToArray();
		}

		public ObjectPool[] GetUniqueAreaHitFxPools()
		{
			return _projectilePools
				.Where( pp => pp.AreaHitFxPool != null )
				.Select( pp => pp.AreaHitFxPool )
				.Distinct()
				.ToArray();
		}


		/*public ObjectPool GetFlyPool( string weaponName )
		{
			var index = _projectilePools.FindIndex( data => string.Equals( data.Name, weaponName ) );
			//Debug.Log($"Head material index : {index} for {itemName}"  );
			
			return index == -1 ? null : _projectilePools[index].FlyFxPool;
		}*/

		public ObjectPool GetFlyPool( uint weaponId )
		{
			var index = _projectilePools.FindIndex( data => data.ID == weaponId );
			//Debug.Log($"Head material index : {index} for {itemName}"  );

			return index == -1 ? null : _projectilePools[index].FlyFxPool;
		}

		public ObjectPool GetHitFxPool( uint weaponId )
		{
			var index = _projectilePools.FindIndex( data => data.ID == weaponId );
			//Debug.Log($"Head material index : {index} for {itemName}"  );

			return index == -1 ? null : _projectilePools[index].HitFxPool;
		}

		public ObjectPool GetHitDecalPool( uint weaponId )
		{
			var index = _projectilePools.FindIndex( data => data.ID == weaponId );
			//Debug.Log($"Head material index : {index} for {itemName}"  );

			return index == -1 ? null : _projectilePools[index].HitDecalPool;
		}

		public ObjectPool GetAreaHitFxPool( uint weaponId )
		{
			var index = _projectilePools.FindIndex( data => data.ID == weaponId );
			//Debug.Log($"Head material index : {index} for {itemName}"  );

			return index == -1 ? null : _projectilePools[index].AreaHitFxPool;
		}


		private void OnValidate()
		{
			#if UNITY_EDITOR
			var path = "Assets/_Game/_Configs/ItemsLibrary.asset";
			_itemsLibrary = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemsLibrary>( path );
			#endif

			if ( _itemsLibrary == null )
				return;
			
			for ( var i = 0; i < _projectilePools.Count; i++ )
			{
				var record = _projectilePools[i];

				ValidateRecord( ref record );

				_projectilePools[i] = record;
			}
		}


		private void ValidateRecord(ref WeaponProjectilePoolRecord record)
		{
			var stack = new InventoryItemsStack( new InventoryItem(record.ID, record.Name), 1 );

			if ( stack.ItemID == 0 && string.IsNullOrEmpty( stack.ItemName ) )
			{
				return;
			}

			if ( stack.ItemID == 0 )
			{
				stack = _itemsLibrary.RestoreID_2( stack );

				if ( stack.ItemID == 0 )
					Debug.LogWarning( $"item {stack.ItemName} not found for {name}" );
			}
			else
			{
				var restoredItem = _itemsLibrary.RestoreName_2( stack );

				if ( string.IsNullOrEmpty( restoredItem.ItemName ) == false )
					stack = restoredItem;
			}

			if ( stack.ItemID == 0 || _itemsLibrary.IsItemInCategory( stack.ItemID, ItemsCategories.RangeWeaponCategory ) == false )
			{
				stack = new InventoryItemsStack();
			}
			else
			{
			}

			record = new WeaponProjectilePoolRecord( stack.ItemID, stack.ItemName, record.FlyFxPool, record.HitFxPool, record.HitDecalPool, record.AreaHitFxPool );
		}
	}
}