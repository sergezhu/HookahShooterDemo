namespace _Game._Scripts.Managers.Input
{
	using _Game._Scripts.Utilities;
	using _Game._Scripts.Utilities.Extensions;
	using Sirenix.OdinInspector;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;

	[RequireComponent(typeof(RectTransform))]
	public class RectTransformScreenArea : MonoBehaviour
	{
		[SerializeField] private CanvasScaler _canvasScaler;
		[SerializeField] private RectTransform _canvasScalerRT;

		[Space]
		[SerializeField] private float _paddingRelativeLeft;
		[SerializeField] private float _paddingRelativeRight;
		[SerializeField] private float _paddingRelativeTop;
		[SerializeField] private float _paddingRelativeBottom;
		
		private ScreenResizeDetector _screenResizeDetector;
		private RectTransform _rectTransform;

		public float PaddingRelativeLeft => _paddingRelativeLeft;
		public float PaddingRelativeRight => _paddingRelativeRight;
		public float PaddingRelativeTop => _paddingRelativeTop;
		public float PaddingRelativeBottom => _paddingRelativeBottom;

		[Inject]
		private void Construct( ScreenResizeDetector screenResizeDetector )
		{
			_screenResizeDetector = screenResizeDetector;
			_rectTransform = GetComponent<RectTransform>();

			_screenResizeDetector.Size
				.Subscribe( size => OnScreenSizeChanged( size ) )
				.AddTo( this );
		}

		private void OnScreenSizeChanged( Vector2Int size )
		{
			//var scale = _canvasScalerRT.localScale;
			//var scaledSizeX = size.x / scale.x;
			//var scaledSizeY = size.y / scale.y;

			var canvasRTSize = _canvasScalerRT.rect.size;
			var scaledSizeX = canvasRTSize.x;
			var scaledSizeY = canvasRTSize.y;

			var xMinRaw = scaledSizeX * _paddingRelativeLeft;
			var xMaxRaw = scaledSizeX * (1f - _paddingRelativeRight);
			var xMin = Mathf.Min( xMinRaw, xMaxRaw );
			var xMax = Mathf.Max( xMinRaw, xMaxRaw );

			var yMinRaw = scaledSizeY * _paddingRelativeBottom;
			var yMaxRaw = scaledSizeY * (1f - _paddingRelativeTop);
			var yMin = Mathf.Min( yMinRaw, yMaxRaw );
			var yMax = Mathf.Max( yMinRaw, yMaxRaw );

			var w = xMax - xMin;
			var h = yMax - yMin;
			
			//var w = 0.5f * scaledSizeX;
			//var h = size.y / scale.y;
			
			//var w = 0.5f * size.x;
			//var h = size.y;	

			_rectTransform.anchoredPosition = new Vector2( xMin, yMin );
			_rectTransform.SetSize( w, h );
			
			Debug.Log( $"screen {size.x}x{size.y}, scaled screen {scaledSizeX}x{scaledSizeY}, " +
					   $"screen res {Screen.currentResolution.width}x{Screen.currentResolution.height}, " +
					   $"canvasScaler {_canvasScalerRT.rect.width}x{_canvasScalerRT.rect.height}, " +
					   $"ref {w}x{h}, real {_rectTransform.rect.width} {_rectTransform.rect.height}" );
		}

		[Button]
		private void Apply()
		{
			_rectTransform = GetComponent<RectTransform>();
			OnScreenSizeChanged( new Vector2Int( Screen.width, Screen.height ) );
		}
	}
}	