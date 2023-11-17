using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace _Game._Scripts.StateMachine.Base
{
    public abstract class BaseAIStateMachine
    {
        protected Dictionary<Type, IAIState> _states;

        protected ReactiveProperty<IAIState> _currentState;
        public ReadOnlyReactiveProperty<IAIState> CurrentState { get; }

        protected BaseAIStateMachine()
        {
            _states = new Dictionary<Type, IAIState>();

            _currentState = new ReactiveProperty<IAIState>();
            CurrentState = _currentState.ToReadOnlyReactiveProperty();
        }

        public void Tick()
        {
            if ( _currentState.Value != null )
                _currentState.Value.Update();
        }

        public void FixedTick()
        {
            if ( _currentState.Value != null )
                _currentState.Value.FixedUpdate();
        }

        public virtual void Start()
        {
            SetState( _states.Values.First() );
        }

        public virtual void Stop()
        {
            ResetState();
        }

        public void AddState( IAIState state )
        {
            if(_states.Values.Contains( state ))
                return;
        
            _states.Add( state.GetType(), state );
        }

        public void AddState<TState>() where TState : IAIState, new()
        {
            if ( _states.Keys.Contains( typeof(TState) ) )
                return;

            _states.Add( typeof(TState), new TState() );
        }

        public void SetState(IAIState newState)
        {
            if (_currentState.Value != null) 
                _currentState.Value.Exit();

            _currentState.Value = newState;
            _currentState.Value.Enter();
        }

        public void SetState<TState>() where TState : IAIState
        {
            if ( _currentState.Value != null )
                _currentState.Value.Exit();

            var targetState = _states[typeof(TState)];

            _currentState.Value = targetState;
            _currentState.Value.Enter();
        }

        public void ResetState()
        {
            if (_currentState.Value != null) 
                _currentState.Value.Exit();
        
            _currentState.Value = null;
        }

        protected IAIState GetState<T>() where T : IAIState
        {
            var type = typeof(T);
            return _states[type];
        }

        public void CleanUp()
        {
            _currentState?.Dispose();
            CurrentState?.Dispose();
        }
    }
}
