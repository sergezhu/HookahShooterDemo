namespace _Game._Scripts.Managers.Input
{
    using Zenject;

    public interface IInputManager
    {
        InputActions.DevCheatsActions DevCheats { get; }
        InputActions.TouchActions Touch { get; }
        InputActions.KeyboardActions Keyboard { get; }
    }
    


    public class InputManager : IInputManager, IInitializable
    {
        private InputActions _actions;

        public InputActions.DevCheatsActions DevCheats { get; private set; }
        public InputActions.TouchActions Touch { get; private set; }
        public InputActions.KeyboardActions Keyboard { get; private set; }
        
        public void Initialize()
        {
            _actions = new InputActions();
            
            DevCheats = _actions.DevCheats;
            Touch = _actions.Touch;
            Keyboard = _actions.Keyboard;
            
            //Touch.Enable();

            #if UNITY_EDITOR
            
            Keyboard.Enable();
            DevCheats.Enable();
            
            #endif
        }
    }
}