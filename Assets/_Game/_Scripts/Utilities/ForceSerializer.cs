namespace _Game._Scripts.Utilities
{
	using Sirenix.OdinInspector;
	using UnityEngine;

	public class ForceSerializer : MonoBehaviour
	{
		[SerializeField] private string[] _assetPaths;

		[Button]
		private void ForceReserialize()
		{
			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.ForceReserializeAssets(_assetPaths);
			#endif
		}
	}
}