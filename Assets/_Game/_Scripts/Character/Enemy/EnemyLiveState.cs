namespace _Game._Scripts.Character.Enemy
{
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class EnemyLiveState : IInitializable
	{
		private readonly EnemyConfig _enemyConfig;
		private readonly CompositeDisposable _disposable;

		public EnemyLiveState( EnemyConfig enemyConfig )
		{
			_enemyConfig = enemyConfig;
			_disposable = new CompositeDisposable();
		}

		public float RemaidedOnConsumeInt { get; private set; }


		public ReactiveProperty<float> Health { get; } = new ReactiveProperty<float>();

		public bool IsDead => Health.Value <= 0;

		public void Initialize()
		{
			Health.Value = _enemyConfig.MaxHealth;
			Health.Subscribe( _ => ConsumeInt() ).AddTo( _disposable );
		}

		public void CleanUp()
		{
			_disposable.Clear();
			Health?.Dispose();
		}

		public void ConsumeInt()
		{
			RemaidedOnConsumeInt = Health.Value - Mathf.Round( Health.Value );
		}
	}
}