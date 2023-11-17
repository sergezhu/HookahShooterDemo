namespace _Game._Scripts.Character.Hero
{
	using _Game._Scripts.Character.Unit;
	using _Game._Scripts.Configs;
	using DG.Tweening;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IHeroLiveState : IUnitLiveState
	{
	}
	
	public class HeroLiveState: IInitializable, IHeroLiveState
	{
		private readonly HeroConfig _heroConfig;
		private readonly CompositeDisposable _disposable;

		public HeroLiveState(HeroConfig heroConfig )
		{
			_heroConfig = heroConfig;
			_disposable = new CompositeDisposable();

			Health = _health.ToReadOnlyReactiveProperty();
		}

		private Tween _tween;
		private float _targetHealth;
		private float _targetEnergy;
		private float _currentHealth;
		private float _currentEnergy;

		public float RemaidedOnConsumeInt { get; private set; }
		public bool IsHealthFull => _health.Value >= _heroConfig.MaxHealth - 1e-6;

		public float LastHealthDelta { get; private set; }

		public ReadOnlyReactiveProperty<float> Health { get; }
		private ReactiveProperty<float> _health { get; } = new ReactiveProperty<float>();


		public void Initialize()
		{
			_health.Value = _heroConfig.MaxHealth * _heroConfig.DebugStartHealthModifier;

			Health.Subscribe( _ => ConsumeInt() ).AddTo( _disposable );
		}

		public void ChangeHealth( float delta )
		{
			var prevHealth = _health.Value;
			_health.Value = Mathf.Clamp( _health.Value + delta, 0, _heroConfig.MaxHealth );

			LastHealthDelta = _health.Value - prevHealth;
		}

		public void Ressurect()
		{
			_health.Value = _heroConfig.RessurectHealth;
		}


		public bool IsDead => Health.Value <= 0;

		public void CleanUp()
		{
			_tween?.Kill();
			_disposable.Clear();
		}

		private void ConsumeInt()
		{
			RemaidedOnConsumeInt = Health.Value - Mathf.Round( Health.Value );
		}

		public void TweenHealthBy( int healthDelta )
		{
			_currentHealth = Health.Value;
			_targetHealth = Mathf.Clamp( Health.Value + healthDelta, 0, _heroConfig.MaxHealth );

			_tween?.Kill();

			_tween = DOVirtual.Float( 0f, 1f, _heroConfig.LiveParametersTweenDuration, v =>
				{
					_health.Value = Mathf.Lerp( _currentHealth, _targetHealth, v );
				} )
				.SetEase( Ease.OutQuad )
				.OnComplete( () => _tween = null );
		}
	}
}