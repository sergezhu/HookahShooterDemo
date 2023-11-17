namespace _Game._Scripts.Managers.LevelsManagement
{
	using _Game._Scripts.Managers.LevelsManagement.LevelTypes;
	using _Game._Scripts.MapGenerator;
	using UnityEngine;
	using Zenject;

	public class LevelsFactory
	{
		private readonly LevelsLibrary _levelsLibrary;
		private readonly IInstantiator _instantiator;

		public LevelsFactory(LevelsLibrary levelsLibrary, IInstantiator instantiator)
		{
			_levelsLibrary = levelsLibrary;
			_instantiator = instantiator;
		}

		public ILevel CreateLevel( Transform levelsRoot = null )
		{
			var levelPrefab = _levelsLibrary.LevelPrefab;

			Debug.Log( $"[LevelFactory] Create Level " );
			
			var instantiatedLevel = _instantiator
				.InstantiatePrefab( levelPrefab, levelPrefab.transform.position, Quaternion.identity, levelsRoot )
				.GetComponent<ILevel>();

			return instantiatedLevel;
		}
	}
}