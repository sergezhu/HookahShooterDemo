namespace _Game._Scripts.Managers.LevelsManagement
{
	using UniRx;
	using UnityEngine.AI;
	using Zenject;

	public class NavMeshProvider : IInitializable
	{
		private readonly LevelsController _levelsController;
		private readonly CompositeDisposable _disposable;
		private NavMeshSurface _currentNavSurface;

		public NavMeshProvider(LevelsController levelsController)
		{
			_levelsController = levelsController;
			_disposable = new CompositeDisposable();
		}

		public NavMeshSurface CurrentNavSurface => _currentNavSurface;

		public void Initialize()
		{
			_levelsController.LifeCycleState
				.Subscribe( state => 
				{
					_currentNavSurface = state == LevelsController.LifeState.Loaded 
								? _levelsController.CurrentLevel.NavMeshSurface 
								: null; 
				} )
				.AddTo( _disposable );
		}
	}
}