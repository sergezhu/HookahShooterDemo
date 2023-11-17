namespace _Game._Scripts.Managers.Input
{
	using _Game._Scripts.Level;
	using _Game._Scripts.Utilities.Extensions;
	using Zenject;

	public class DevInput : IInitializable
    {
        [Inject] private IInputManager _inputManager;
        [Inject] private SystemPauseService _systemPauseService;

        private InputActions.DevCheatsActions DevActions => _inputManager.DevCheats;

        public void Initialize()
        {
			#if UNITY_EDITOR
			DevActions.PauseToggle.SubscribeToPerformed( ctx => _systemPauseService.ToggleTimescalePause() );
			#endif
		}
    }
}