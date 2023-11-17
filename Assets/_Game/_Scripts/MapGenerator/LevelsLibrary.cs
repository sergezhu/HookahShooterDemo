namespace _Game._Scripts.MapGenerator
{
	using _Game._Scripts.Managers.LevelsManagement.LevelTypes;
	using UnityEngine;

	[CreateAssetMenu( fileName = "LevelsLibrary", menuName = "Configs/LevelsLibrary" )]
	public class LevelsLibrary : ScriptableObject
	{
		[SerializeField] private Level _levelPrefab;

		public Level LevelPrefab => _levelPrefab;
	}
}