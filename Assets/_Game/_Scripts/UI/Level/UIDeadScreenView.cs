namespace _Game._Scripts.UI.Level
{
	using _Game._Scripts.UI.Base;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UIDeadScreenView : MonoBehaviour, IInitializable
	{
		[SerializeField] private UIBaseButton _restartOnCurrentButton;
		
		public ReactiveCommand RestartClick { get; private set; }

		public void Initialize()
		{
			_restartOnCurrentButton.Initialize();

			RestartClick = _restartOnCurrentButton.Click;
		}
	}
}