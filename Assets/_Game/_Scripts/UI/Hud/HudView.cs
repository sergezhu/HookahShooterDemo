namespace _Game._Scripts.UI.Hud
{
	using System;
	using _Game._Scripts.UI.Base;
	using _Game._Scripts.Utilities.Extensions;
	using Sirenix.Utilities;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;


	public interface IHudView : IInitializable
	{
		bool IsAttackButtonEnabled { get; }
		IObservable<Unit> AttackButtonClick { get; }
		ReactiveProperty<bool> IsAttackButtonPressed { get; }
		IObservable<Unit> RestartButtonClick { get; }

		void Hide();
		void Show();
		void EnableAttackButton();
		void DisableAttackButton();
		void ClearWeaponButtonIcon();
		void SetWeaponButtonIcon( Sprite icon );
		void SetAttackButtonInRangeState( bool inRange );


		void SetHealth( float currentValue, float maxValue );
		void SetAttackButtonProgress( float value );
	}

	public class HudView : MonoBehaviour, IHudView
	{
		[SerializeField] private HudAttackButton _attackButton;
		[SerializeField] private UIBaseButton _restartButton;
		
		[SerializeField, Space] private TextMeshProUGUI _healthText;
		[SerializeField] private Image _healthProgressImage;

		public bool IsAttackButtonEnabled => _attackButton.IsEnabled.Value;
		public IObservable<Unit> AttackButtonClick { get; private set; }
		public IObservable<Unit> RestartButtonClick { get; private set; }
		public ReactiveProperty<bool> IsAttackButtonPressed { get; private set; }


		public void Initialize()
		{
			var buttons = new UIBaseButton[] { _attackButton, _restartButton };
			LinqExtensions.ForEach( buttons, b => b.Initialize() );

			AttackButtonClick = _attackButton.Click;
			IsAttackButtonPressed = _attackButton.IsPressed;
			RestartButtonClick = _restartButton.Click;

			_attackButton.SetProgress( 0 );
		}

		public void Hide() => MonoExtensions.Hide( this );
		public void Show() => MonoExtensions.Show( this );

		public void EnableAttackButton() => _attackButton.IsEnabled.Value = true;
		public void DisableAttackButton() => _attackButton.IsEnabled.Value = false;

		public void ClearWeaponButtonIcon() => _attackButton.ClearIcon();
		public void SetWeaponButtonIcon( Sprite icon ) => _attackButton.SetIcon( icon );
		public void SetAttackButtonInRangeState( bool state ) => _attackButton.SetAttackButtonInRangeState( state );
		public void SetAttackButtonProgress( float value ) => _attackButton.SetProgress( value );


		public void SetHealth( float currentValue, float maxValue )
		{
			var curentHealthInt = Mathf.Round( currentValue );
			var maxValueInt = Mathf.Round( maxValue );

			_healthText.text = $"{curentHealthInt}/{maxValueInt}";
			_healthProgressImage.fillAmount = currentValue / maxValue;
		}
	}
}