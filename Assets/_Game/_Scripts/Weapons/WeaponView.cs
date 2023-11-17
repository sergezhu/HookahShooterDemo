namespace _Game._Scripts.Weapons
{
	using _Game._Scripts.InventoryItemsServices.WorldItemViews;
	using UnityEngine;

	public interface IWeaponView
	{
		string ItemName { get; }
		Transform ShootPoint { get; }
	}
	
	[RequireComponent(typeof(MeshItemView))]
	public class WeaponView : MonoBehaviour, IWeaponView
	{
		[SerializeField] private MeshItemView _meshItemView;
		[SerializeField] private Transform _shootPoint;

		public string ItemName => _meshItemView.ItemName;
		public Transform ShootPoint => _shootPoint;

		private void OnValidate()
		{
			_meshItemView = GetComponent<MeshItemView>();
		}
	}
}