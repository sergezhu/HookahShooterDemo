namespace _Game._Scripts.Utilities
{
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class ScreenResizeDetector : IInitializable, ITickable
	{
		public ReactiveProperty<Vector2Int> Size { get; } = new ReactiveProperty<Vector2Int>();

		
		public void Initialize()
		{
			UpdateCurrentScreenSize();
		}

		private void UpdateCurrentScreenSize()
		{
			Size.Value = new Vector2Int( Screen.width, Screen.height );
		}

		public void Tick()
		{
			UpdateCurrentScreenSize();
		}
	}
}