namespace _Game._Scripts.Utilities.EditorHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Sirenix.OdinInspector;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class MissingScriptsFinder : MonoBehaviour
	{
		private const string PREFAB_MISSING_TEXT = "Missing Prefab";
		
		[SerializeField] private List<GameObject> _gameObjectsWithMissingScripts;
		[SerializeField] private List<GameObject> _gameObjectsWithMissingPrefabs;

		#if UNITY_EDITOR
		[Button]
		private void FindMissingScripts()
		{
			var gameObjectsWithMissingScripts = GetAllGameObjects( SceneManager.GetActiveScene() )
				.Where( HasMissingScripts )
				.ToList();

			var gameObjectsNamesWithMissingScripts =
				gameObjectsWithMissingScripts
					.GroupBy( gameObject => gameObject.name )
					.Select( grouping => $"{grouping.Key} has [{grouping.Count()}] missing prefabs" )
					.ToList();

			_gameObjectsWithMissingScripts = gameObjectsWithMissingScripts;
		}

		[Button]
		private void FindMissingPrefabs()
		{
			var allPrefabs = GetAllPrefabParts( SceneManager.GetActiveScene() ).ToList();

			List<GameObject> missingPrefabs = allPrefabs
				.Where( IsMissingPrefab )
				.GroupBy( gameObject => gameObject )
				.Select( grouping => grouping.Key )
				.ToList();
			
			List<string> missingPrefabsNames =
				allPrefabs
					.Where( IsMissingPrefab )
					.GroupBy( gameObject => gameObject.name )
					.Select( grouping => $"{grouping.Key} is [{grouping.Count()}] missing prefabs" )
					.ToList();

			var probablyInvalidPrefabChildren =
				allPrefabs
					.Where( gameObject => IsProbablyMissingChild( gameObject ) && missingPrefabsNames.Contains( gameObject.name ) == false )
					.GroupBy( gameObject => gameObject.name )
					.Select( grouping => $"{grouping.Key} is [{grouping.Count()}] probably invalid prefab child" )
					.ToList();

			_gameObjectsWithMissingPrefabs = missingPrefabs;
		}

		private static IEnumerable<GameObject> GetAllGameObjects( Scene scene )
		{
			var gameObjectQueue = new Queue<GameObject>( scene.GetRootGameObjects() );

			while ( gameObjectQueue.Count > 0 )
			{
				var gameObject = gameObjectQueue.Dequeue();

				yield return gameObject;

				foreach ( Transform child in gameObject.transform )
					gameObjectQueue.Enqueue( child.gameObject );
			}
		}

		private static IEnumerable<GameObject> GetAllPrefabParts( Scene scene )
		{
			var allGamObjects = GetAllGameObjects( scene );
			var allPrefabs = allGamObjects.Where( UnityEditor.PrefabUtility.IsPartOfAnyPrefab );

			foreach ( var prefab in allPrefabs )
			{
				yield return prefab;
			}
		}

		private static IEnumerable<Scene> ActiveScene() =>
			new List<Scene>() {SceneManager.GetActiveScene()};

		private static IEnumerable<string> ActiveScenePath() =>
			new List<string>() {SceneManager.GetActiveScene().path};

		static bool HasMissingScripts( GameObject gameObject ) =>
			UnityEditor.GameObjectUtility.GetMonoBehavioursWithMissingScriptCount( gameObject ) > 0;

		static bool IsMissingPrefab( GameObject gameObject ) =>
			UnityEditor.PrefabUtility.IsPrefabAssetMissing( gameObject );

		static bool IsProbablyMissingChild( GameObject gameObject )
		{
			var gameObjectName = gameObject.name;
			var hasMissingText = gameObjectName.IndexOf( PREFAB_MISSING_TEXT, StringComparison.Ordinal ) != -1;
			return hasMissingText;
		}
		#endif
	}
}