namespace _Game._Scripts.Character.Unit
{
	using _Game._Scripts.Weapons;
	using UnityEngine;

	public interface IUnitView
	{
		Vector3 Position { get; set; }
		Quaternion Rotation { get; }
		Vector3 Forward { get; }
		Vector3 Right { get; }
		Vector3 AimTargetPosition { get; }
		string Name { get; }

		WeaponView GetWeaponViewByName( string weaponName );
	}
}