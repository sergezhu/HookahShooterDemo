namespace _Game._Scripts.Managers.LevelsManagement.LevelTypes
{
	using _Game._Scripts.Character;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Level;
	using _Game._Scripts.Utilities.Extensions;
	using Sirenix.OdinInspector;
	using UnityEngine;
	using UnityEngine.AI;
	using Zenject;

	[SelectionBase]
	public class Level : MonoBehaviour, ILevel
	{
		private NavMeshSurface _navMeshSurface;
		private LevelTargetsProvider _levelTargetsProvider;
		private HeroRoot _heroRoot;
		private UnitsFactory _unitsFactory;

		public string Name => name;
		
		public NavMeshSurface NavMeshSurface => _navMeshSurface;


		[Inject]
		public void Construct(UnitsFactory unitsFactory, NavMeshSurface navMeshSurface, LevelTargetsProvider levelTargetsProvider, HeroRoot heroRoot)
		{
			_unitsFactory = unitsFactory;
			_navMeshSurface = navMeshSurface;
			_levelTargetsProvider = levelTargetsProvider;
			_heroRoot = heroRoot;
		}

		public Vector3 GetHeroSpawnPosition()
		{
			
			return _heroRoot.Position;
		}

		[Button]
		public void SpawnHero()
		{
			var instantiatedHero = _unitsFactory.CreateHero( GetHeroSpawnPosition().WithY( 0 ), Quaternion.identity, _heroRoot.Transform );
			_levelTargetsProvider.AddTarget( instantiatedHero );
		}
	}
}