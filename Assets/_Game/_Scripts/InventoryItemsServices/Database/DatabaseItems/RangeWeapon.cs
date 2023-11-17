namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItems
{
	using System;
	using _Game._Scripts.Enums;
	using _Game._Scripts.InventoryItemsServices.Database.AttributesData;
	using UnityEngine;

	[Serializable]
	public class RangeWeapon : Weapon
	{
		[SerializeField] private RangeWeaponAttributes _rangeWeaponAttributes;

		public int QueueLength => _rangeWeaponAttributes.QueueLength;
		public float BetweenQueueShotsDelay => _rangeWeaponAttributes.BetweenQueueShotsDelay;
		
		public int SeparateDamageCount => _rangeWeaponAttributes.SeparateDamageCount;
		public float RandomAngleScatter => _rangeWeaponAttributes.RandomAngleScatter;
		public float StaticAngleScatter => _rangeWeaponAttributes.StaticAngleScatter;
		public float OwnerRotateCompensationY => _rangeWeaponAttributes.OwnerRotateCompensationY;

		public bool HitAreaDamageEnabled => _rangeWeaponAttributes.HitAreaDamageEnabled;
		public float HitAreaDamageRadius => _rangeWeaponAttributes.HitAreaDamageRadius;
		public float HitAreaDamageValue => _rangeWeaponAttributes.HitAreaDamageValue;
		

		public RangeWeapon( uint id, string name, BaseWeaponAttributes baseWeaponAttributes, RangeWeaponAttributes rangeWeaponAttributes, Sprite icon ) 
			: base( id, name, baseWeaponAttributes, icon )
		{
			_rangeWeaponAttributes = rangeWeaponAttributes;
		}
	}
}