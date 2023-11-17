namespace _Game._Scripts.Managers.Input
{
    using _Game._Scripts.Utilities.Extensions;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class KeyboardInput : IInitializable
    {
        [Inject] private IInputManager _inputManager;


        private InputActions.KeyboardActions KeyboardActions => _inputManager.Keyboard;

        public ReactiveProperty<Vector2> MoveDirection { get; } = new ReactiveProperty<Vector2>( Vector2.zero );
        
        public void Initialize()
        {
            WASDSubscribe();
        }

        void WASDSubscribe()
        {
            KeyboardActions.MoveForwardPress.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( 0, 1 ); } );
            KeyboardActions.MoveForwardRelease.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( 0, -1 ); } );

            KeyboardActions.MoveBackwardPress.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( 0, -1 ); } );
            KeyboardActions.MoveBackwardRelease.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( 0, 1 ); } );

            KeyboardActions.MoveRightPress.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( 1, 0 ); } );
            KeyboardActions.MoveRightRelease.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( -1, 0 ); } );

            KeyboardActions.MoveLeftPress.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( -1, 0 ); } );
            KeyboardActions.MoveLeftRelease.SubscribeToPerformed( ctx => { MoveDirection.Value += new Vector2( 1, 0 ); } );
        }
    }
}