namespace _Game._Scripts.UI.Hud
{
	using System;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Configs;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using _Game._Scripts.Managers.LevelsManagement;
	using UniRx;
	using Zenject;

	public class HudController : IInitializable
	{
		private readonly IHudView _view;
		private readonly LevelsController _levelsController;
		private readonly HeroConfig _heroConfig;
		private readonly CompositeDisposable _disposable;
		
		private Weapon _currentWeapon;

		public IObservable<Unit> AttackButtonClick { get; private set; }
		public IObservable<Unit> RestartButtonClick { get; private set; }
		public IObservable<bool> IsAttackButtonPressed { get; private set; }

		private bool IsEnabled { get; set; }

		public HudController(IHudView view, LevelsController levelsController )
		{
			_view = view;
			_levelsController = levelsController;

			_disposable = new CompositeDisposable();
		}

		public void Initialize()
		{
			AttackButtonClick = _view.AttackButtonClick.Where( _ => IsEnabled );
			RestartButtonClick = _view.RestartButtonClick.Where( _ => IsEnabled );
			IsAttackButtonPressed = _view.IsAttackButtonPressed.Where( _ => IsEnabled );

			UpdateButtonsViews();
			Subscribe();
		}

		public void EnableAttackButton()
		{
			if ( _view.IsAttackButtonEnabled )
				return;
			
			_view.EnableAttackButton();
		}

		public void DisableAttackButton()
		{
			if ( _view.IsAttackButtonEnabled == false )
				return;
			
			_view.DisableAttackButton();
		}

		public void SetAttackButtonInRange( bool inRange ) => _view.SetAttackButtonInRangeState( inRange );
		public void SetAttackProgress( float progress ) => _view.SetAttackButtonProgress( progress );


		private void Subscribe()
		{
			_levelsController.LifeCycleState
				.Subscribe( OnChunkManagerStateChanged )
				.AddTo( _disposable );

			_view.RestartButtonClick
				.Where( _ => _levelsController.LifeCycleState.Value == LevelsController.LifeState.Loaded )
				.Subscribe( _ => _levelsController.ReloadCurrentLevel() )
				.AddTo( _disposable );
		}

		private void OnChunkManagerStateChanged( LevelsController.LifeState state )
		{
			IsEnabled = state == LevelsController.LifeState.Loaded;

			if ( IsEnabled == false )
			{
				_view.IsAttackButtonPressed.Value = false;
				_view.Hide();
			}
			else
			{
				_view.Show();
			}
		}

		public void OnActiveWeaponChanged( Weapon weapon )
		{
			_currentWeapon = weapon;
			
			UpdateButtonsViews();
		}

		private void UpdateButtonsViews()
		{
			if ( _currentWeapon == null )
				_view.ClearWeaponButtonIcon();
			else
				_view.SetWeaponButtonIcon( _currentWeapon.Icon );
		}

		public void SetHealth( float currentValue, float maxValue )
		{
			_view.SetHealth( currentValue, maxValue );
		}
	}
}	