namespace _Game._Scripts.UI.Base
{
	using _Game._Scripts.Utilities.Extensions;
	using Sirenix.OdinInspector;
	using UnityEngine;
	using UnityEngine.UI;

	public class UIImageButton : UIBaseButton
	{
		[SerializeField, Space] private Image _defaultIconImage;
		[SerializeField] private Image _customIconImage;
		
		[SerializeField, Space] private RectTransform _lockLayer;
		[SerializeField, ReadOnly] private bool _isLocked;

		protected override void CustomInitialize()
		{
			base.CustomInitialize();
			
			ClearIcon();
			SetLockState( false, true );
		}

		public void SetIcon( Sprite icon )
		{
			_customIconImage.sprite = icon;
			_customIconImage.Show();
			
			if(_defaultIconImage != null)
				_defaultIconImage.Hide(); 
		}

		public void ClearIcon()
		{
			_customIconImage.Hide();
			SetIconOpacity( 1 );

			if(_defaultIconImage != null)
				_defaultIconImage.Show();
		}

		public Sprite GetIcon()
		{
			return _customIconImage.sprite;
		}

		public void SetIconOpacity( float opacity )
		{
			opacity = Mathf.Clamp( opacity, 0f, 1f );
			var color = Color.white;
			color.a = opacity;

			_customIconImage.color = color;
		}

		public void SetLockState( bool isLocked, bool clearIfLocked )
		{
			_isLocked = isLocked;
			
			if ( _lockLayer == null )
				return;

			if ( isLocked )
			{
				if( clearIfLocked )
					ClearIcon();
				
				_lockLayer.Show();
			}
			else
			{
				_lockLayer.Hide();
			}
		}
	}
}