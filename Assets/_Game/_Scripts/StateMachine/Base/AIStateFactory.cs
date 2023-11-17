namespace _Game._Scripts.StateMachine.Base
{
	using Zenject;

	public class AIStateFactory
	{
		private readonly IInstantiator _instantiator;

		public AIStateFactory(IInstantiator instantiator)
		{
			_instantiator = instantiator;
		}

		public TState CreateState<TState>() where TState : IAIState
		{
			return _instantiator.Instantiate<TState>();
		}
	}
}