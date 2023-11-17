namespace _Game._Scripts.UI.Hud
{
	using _Game._Scripts.Utilities.Extensions;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class UnitHealthView : MonoBehaviour
	{
		[SerializeField] private Image _progressImage;
		[SerializeField] private TextMeshProUGUI _healthAmountText;
		[SerializeField] private bool _hideIfDead;
		[SerializeField] private bool _hideIfFull;
		
		private float _maxHealth;
		private float _currentHealth;
		private bool _isShown;

		public void Construct( float maxHealth )
		{
			_maxHealth = maxHealth;
			_currentHealth = maxHealth;

			_isShown = false;
			this.Hide();

			UpdateView();
		}

		public void SetHealth( float health )
		{
			_currentHealth = health;
			
			UpdateView();
		}

		private void UpdateView()
		{
			float progress = _currentHealth / _maxHealth;
			_progressImage.fillAmount = progress;

			var roundedHealth = Mathf.CeilToInt( _currentHealth );
			_healthAmountText.text = $"{roundedHealth}";

			var isDead = roundedHealth == 0;
			var isFull = roundedHealth == Mathf.RoundToInt( _maxHealth );

			if ( isDead && _hideIfDead || isFull && _hideIfFull )
			{
				if ( _isShown )
				{
					_isShown = false;
					this.Hide();
				}
				
			}
			else
			{
				if ( _isShown == false )
				{
					_isShown = true;
					this.Show();
				}
			}
		}
	}
}