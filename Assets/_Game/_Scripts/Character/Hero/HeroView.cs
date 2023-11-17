namespace _Game._Scripts.Character.Hero
{
	using System.Linq;
	using _Game._Scripts.Character.Unit;
	using _Game._Scripts.Configs;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.UI.Hud;
	using _Game._Scripts.Weapons;
	using UnityEngine;
	using Zenject;

	public interface IHeroView : IUnitView
	{
		Transform CameraTarget { get; }
		void UpdateWeapon( Weapon activeWeapon );
		void SetHealth( float healthValue );
		void PlayRestoreHealthFX();
	}

	public class HeroView : MonoBehaviour, IHeroView
	{
		[SerializeField] private Transform _aimTarget;
		[SerializeField] private Transform _cameraTarget;
		[SerializeField] private UnitHealthView _healthView;

		private Transform _transform;
		private WeaponView[] _weaponViews;
		private HeroConfig _heroConfig;

		[Inject]
		private void Construct( WeaponView[] weaponViews, HeroConfig heroConfig )
		{
			_weaponViews = weaponViews;
			_heroConfig = heroConfig;

			_healthView.Construct( _heroConfig.MaxHealth );
			
			Debug.Log( $"WeaponViews : {_weaponViews.Length}" );
		}

		public string Name => name;
		
		public Vector3 Position
		{
			get => Transform.position;
			set => Transform.position = value;
		}

		public Quaternion Rotation => Transform.rotation;

		public Vector3 AimTargetPosition => _aimTarget.position;
		public Vector3 Forward => Transform.forward;
		public Vector3 Right => Transform.right;

		public Transform CameraTarget => _cameraTarget;


		public void UpdateWeapon( Weapon activeWeapon )
		{
			var activeWeaponName = activeWeapon != null ? activeWeapon.Name : "";
			Debug.Log( $"Update Equip View, weapon : [{activeWeaponName}]" );
		}

		public WeaponView GetWeaponViewByName( string weaponName )
		{
			return _weaponViews.FirstOrDefault( view => string.Equals( view.ItemName, weaponName ) );
		}

		public void SetHealth( float healthValue )
		{
			_healthView.SetHealth( healthValue );
		}

		public void PlayRestoreHealthFX()
		{
		}
		
		private Transform Transform => _transform ??= transform;
	}
}