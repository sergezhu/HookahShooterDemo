namespace _Game._Scripts.Weapons
{
	using System;
	using System.Collections.Generic;
	using _Game._Scripts.InventoryItemsServices;
	using _Game._Scripts.InventoryItemsServices.Static;
	using QFSW.MOP2;
	using Sirenix.OdinInspector;
	using UnityEngine;
	using UnityEngine.Serialization;

	[Serializable]
	public struct WeaponProjectilePoolRecord
	{
		[SerializeField, ReadOnly] private uint _id;
		[ValueDropdown("GetDefinitionsNames")]
		[InlineButton( "ResetItem", "Reset" )]
		[SerializeField] private string _name;
		[FormerlySerializedAs( "_flyPool" )] [FormerlySerializedAs( "_pool" )] 
		[SerializeField] private ObjectPool _flyFxPool;
		[SerializeField] private ObjectPool _hitFxPool;
		[SerializeField] private ObjectPool _hitDecalPool;
		[SerializeField] private ObjectPool _areaHitFxPool;

		public WeaponProjectilePoolRecord( uint id, string name, ObjectPool flyFxPool, ObjectPool hitFxPool,  ObjectPool hitDecalPool, ObjectPool areaHitFxPool )
		{
			_id = id;
			_name = name;
			_flyFxPool = flyFxPool;
			_hitFxPool = hitFxPool;
			_hitDecalPool = hitDecalPool;
			_areaHitFxPool = areaHitFxPool;
		}

		public uint ID => _id;
		public string Name => _name;
		public ObjectPool FlyFxPool => _flyFxPool;
		public ObjectPool HitFxPool => _hitFxPool;
		public ObjectPool HitDecalPool => _hitDecalPool;
		public ObjectPool AreaHitFxPool => _areaHitFxPool;

		private IEnumerable<string> GetDefinitionsNames() => InventoryUtility.GetDefinitionsNames( new[] { ItemsCategories.RangeWeaponCategory } );

		public void SetID( uint id ) => _id = id;

		private void ResetItem()
		{
			_id = 0;
			_name = string.Empty;
		}
	}
}