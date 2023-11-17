namespace _Game._Scripts.InventoryItemsServices.Database.AttributesData
{
	using System;
	using UnityEngine;

	[Serializable]
	public struct RangeWeaponAttributes
	{
		[Min(1)] public int QueueLength;
		public float BetweenQueueShotsDelay;
		public float RandomAngleScatter;
		public float StaticAngleScatter;
		public int SeparateDamageCount;
		public float OwnerRotateCompensationY;
		[HideInInspector] public bool HitAreaDamageEnabled;
		[HideInInspector] public float HitAreaDamageRadius;
		[HideInInspector] public float HitAreaDamageValue;
	}
}