namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItems
{
	using System;
	using _Game._Scripts.Enums;
	using _Game._Scripts.InventoryItemsServices.Database.AttributesData;
	using UnityEngine;
	using Random = UnityEngine.Random;

	[Serializable]
	public abstract class Weapon : DatabaseItem
	{
		[SerializeField] private BaseWeaponAttributes _baseWeaponAttributes;
		
		public float AttackPowerMin => _baseWeaponAttributes.AttackPowerMin;
		public float AttackPowerMax => _baseWeaponAttributes.AttackPowerMax;
		public float AttackDelay => _baseWeaponAttributes.AttackDelay;
		public float AttackDistance => _baseWeaponAttributes.AttackDistance;
		public EWeaponAnimation WeaponAnimation => _baseWeaponAttributes.WeaponAnimation;
		public float WeaponAnimationSpeed => _baseWeaponAttributes.WeaponAnimationSpeed;
		public float HitForceModifier => _baseWeaponAttributes.HitForceModifier;
		public float AveragePower => 0.5f * (AttackPowerMin + AttackPowerMax);
		public float RandomPower => Random.Range(AttackPowerMin, AttackPowerMax);


		public Weapon( uint id, string name, BaseWeaponAttributes baseWeaponAttributes, Sprite icon ) : base( id, name, icon )
		{
			_baseWeaponAttributes = baseWeaponAttributes;
		}
	}
}	