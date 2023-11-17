namespace _Game._Scripts.Enums
{
	using System;

	public static class EnumExt
	{
		public static bool IsMeleeAnimation( this EWeaponAnimation weaponAnimation )
		{
			switch ( weaponAnimation )
			{
				case EWeaponAnimation.None:
					return false;
				case EWeaponAnimation.MeleeZombie:
				case EWeaponAnimation.MeleeRightFist:
				case EWeaponAnimation.MeleeStab:
				case EWeaponAnimation.MeleeOneHanded:
				case EWeaponAnimation.MeleeTwoHanded:
					return true;
				case EWeaponAnimation.Auto:
				case EWeaponAnimation.HandGun:
				case EWeaponAnimation.MiniGun:
				case EWeaponAnimation.Rifle:
				case EWeaponAnimation.Rpg:
				case EWeaponAnimation.Shotgun:
				case EWeaponAnimation.MachineGun:
					return false;
				default:
					throw new ArgumentOutOfRangeException( nameof(weaponAnimation), weaponAnimation, null );
			}
		}

		public static bool IsRangeAnimation( this EWeaponAnimation weaponAnimation )
		{
			switch ( weaponAnimation )
			{
				case EWeaponAnimation.None:
					return false;
				case EWeaponAnimation.MeleeZombie:
				case EWeaponAnimation.MeleeRightFist:
				case EWeaponAnimation.MeleeStab:
				case EWeaponAnimation.MeleeOneHanded:
				case EWeaponAnimation.MeleeTwoHanded:
					return false;
				case EWeaponAnimation.Auto:
				case EWeaponAnimation.HandGun:
				case EWeaponAnimation.MiniGun:
				case EWeaponAnimation.Rifle:
				case EWeaponAnimation.Rpg:
				case EWeaponAnimation.Shotgun:
				case EWeaponAnimation.MachineGun:
					return true;
				default:
					throw new ArgumentOutOfRangeException( nameof(weaponAnimation), weaponAnimation, null );
			}
		}
	}
}