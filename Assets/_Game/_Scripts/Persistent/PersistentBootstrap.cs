namespace _Game._Scripts.Persistent
{
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Zenject;

	public class PersistentBootstrap : MonoBehaviour, IInitializable
	{
		public void Initialize()
		{
			Debug.Log( "[Bootstrap] Editor Load Game Scene" );
			SceneManager.LoadScene( sceneBuildIndex: 1 );
		}
	}
}