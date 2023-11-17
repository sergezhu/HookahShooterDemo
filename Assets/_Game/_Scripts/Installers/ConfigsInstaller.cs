using UnityEngine;
using Zenject;

namespace _Game._Scripts.Installers
{
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Configs;
	using _Game._Scripts.InventoryItemsServices.Database;
	using _Game._Scripts.MapGenerator;
	using _Game._Scripts.Weapons;

	public class ConfigsInstaller : MonoInstaller
	{
		[SerializeField] private LevelsLibrary _levelsLibrary;
		[SerializeField] private UnitsLibrary _unitsLibrary;
		[SerializeField] private InteractionConfig _interactionConfig;
		[SerializeField] private ItemsLibrary _itemsLibrary;
		[SerializeField] private HeroConfig _heroConfig;
		[SerializeField] private BattleConfig _battleConfig;
		[SerializeField] private AIConfig _aiConfig;
		[SerializeField] private ProjectilePools _projectilePools;
		[SerializeField] private CameraSettings _cameraSettings;

		public override void InstallBindings()
		{
			Debug.Log( $"[Install] ConfigsInstaller : InstallBindings" );
		
			Container.BindInstance( _levelsLibrary );
			Container.BindInstance( _interactionConfig );
			Container.BindInstance( _unitsLibrary );
			Container.BindInstance( _itemsLibrary );
			Container.BindInstance( _battleConfig );
			Container.BindInstance( _aiConfig );
			Container.BindInstance( _projectilePools );
			Container.BindInstance( _cameraSettings );
			Container.BindInterfacesAndSelfTo<HeroConfig>().FromInstance( _heroConfig );
		}
	}
}