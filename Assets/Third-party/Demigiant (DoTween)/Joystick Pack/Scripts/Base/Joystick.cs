using _Game._Scripts.Managers.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[SerializeField] private float handleRange = 1;
	[SerializeField] private float deadZone = 0;
	[SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
	[SerializeField] private bool snapX = false;
	[SerializeField] private bool snapY = false;
	
	[SerializeField] private bool isLogsEnabled;
	[SerializeField] private int startDragLogCount;

	[SerializeField] protected RectTransform background = null;
	[SerializeField] private RectTransform handle = null;
	
	[Space]
	[SerializeField] private float _paddingRelativeLeft;
	[SerializeField] private float _paddingRelativeRight;
	[SerializeField] private float _paddingRelativeTop;
	[SerializeField] private float _paddingRelativeBottom;


	private RectTransform _baseRect = null;
	private Canvas _canvas;
	private Camera _cam;
	private Vector2 _input = Vector2.zero;
	private int _currentPonterID = -1;
	private int dragLogCounter = 0;

	private CanvasScaler _canvasScaler;
	private RectTransform _canvasScalerRT;

	public float Horizontal => snapX ? SnapFloat( _input.x, AxisOptions.Horizontal ) : _input.x;
	public float Vertical => snapY ? SnapFloat( _input.y, AxisOptions.Vertical ) : _input.y;
	public Vector2 Direction => new Vector2( Horizontal, Vertical );

	protected bool CanDrag { get; private set; }

	public float HandleRange
	{
		get => handleRange;
		set => handleRange = Mathf.Abs( value );
	}

	public float DeadZone
	{
		get => deadZone;
		set => deadZone = Mathf.Abs( value );
	}

	public AxisOptions AxisOptions
	{
		get => AxisOptions;
		set => axisOptions = value;
	}

	public bool SnapX
	{
		get => snapX;
		set => snapX = value;
	}

	public bool SnapY
	{
		get => snapY;
		set => snapY = value;
	}

	protected virtual void Start()
	{
		isLogsEnabled = false;
		startDragLogCount = 3;
		dragLogCounter = startDragLogCount;

		HandleRange = handleRange;
		DeadZone = deadZone;
		
		_baseRect = GetComponent<RectTransform>();
		_canvas = GetComponentInParent<Canvas>();
		_canvasScaler = _canvas.GetComponent<CanvasScaler>();
		_canvasScalerRT = _canvas.GetComponent<RectTransform>();

		TryGetComponent<RectTransformScreenArea>( out var screenArea );

		_paddingRelativeBottom = screenArea ? screenArea.PaddingRelativeBottom : 0;
		_paddingRelativeTop = screenArea ? screenArea.PaddingRelativeTop : 0;
		_paddingRelativeLeft = screenArea ? screenArea.PaddingRelativeLeft : 0;
		_paddingRelativeRight = screenArea ? screenArea.PaddingRelativeRight : 0.5f;
		
		if ( _canvas == null )
			Debug.LogError( "The Joystick is not placed inside a canvas" );

		var center = new Vector2( 0.5f, 0.5f );
		background.pivot = center;
		handle.anchorMin = center;
		handle.anchorMax = center;
		handle.pivot = center;
		handle.anchoredPosition = Vector2.zero;
	}

	public virtual void OnPointerDown( PointerEventData eventData )
	{
		InitCam();
		
		if(InZone( eventData.position ) == false)
			return;

		CanDrag = true;
		
		if ( isLogsEnabled )
		{
			Debug.Log( $"[Joystick] OnPointerDown, id : {eventData.pointerId}, current_id : {_currentPonterID}" );
			dragLogCounter = startDragLogCount;
		}
		
		_currentPonterID = eventData.pointerId;
		
		OnDrag( eventData );
	}

	public void OnDrag( PointerEventData eventData )
	{
		InitCam();
		
		if(CanDrag == false)
			return;
		
		if ( isLogsEnabled && dragLogCounter > 0 )
		{
			dragLogCounter--;
			Debug.Log( $"[Joystick] OnDrag, id : {eventData.pointerId}, current_id : {_currentPonterID}" );
		}
		
		//if(eventData.pointerId != _currentPonterID)
		//	return;

		var position = RectTransformUtility.WorldToScreenPoint( _cam, background.position );
		var radius = background.sizeDelta / 2;

		_input = (eventData.position - position) / (radius * _canvas.scaleFactor);
		
		FormatInput();
		HandleInput( _input.magnitude, _input.normalized, radius, _cam );
		
		handle.anchoredPosition = _input * radius * handleRange;
	}

	public virtual void OnPointerUp( PointerEventData eventData )
	{
		CanDrag = false;
		
		if ( isLogsEnabled )
		{
			Debug.Log( $"[Joystick] OnPointerUp, id : {eventData.pointerId}, current_id : {_currentPonterID}" );
			dragLogCounter = startDragLogCount;
		}

		//if(eventData.pointerId != _currentPonterID )
		//	return;

		_currentPonterID = -1;
		_input = Vector2.zero;
		
		handle.anchoredPosition = Vector2.zero;
	}

	private void InitCam()
	{
		_cam = null;

		if ( _canvas.renderMode == RenderMode.ScreenSpaceCamera )
			_cam = _canvas.worldCamera;
	}

	protected virtual void HandleInput( float magnitude, Vector2 normalised, Vector2 radius, Camera cam )
	{
		if ( magnitude > deadZone )
		{
			if ( magnitude > 1 )
				_input = normalised;
		}
		else
		{
			_input = Vector2.zero;
		}
	}

	private void FormatInput()
	{
		if ( axisOptions == AxisOptions.Horizontal )
			_input = new Vector2( _input.x, 0f );
		else if ( axisOptions == AxisOptions.Vertical )
			_input = new Vector2( 0f, _input.y );
	}

	private float SnapFloat( float value, AxisOptions snapAxis )
	{
		if ( value == 0 )
			return value;

		if ( axisOptions == AxisOptions.Both )
		{
			var angle = Vector2.Angle( _input, Vector2.up );
			
			if ( snapAxis == AxisOptions.Horizontal )
			{
				if ( angle < 22.5f || angle > 157.5f )
					return 0;
				else
					return value > 0 ? 1 : -1;
			}
			else if ( snapAxis == AxisOptions.Vertical )
			{
				if ( angle > 67.5f && angle < 112.5f )
					return 0;
				else
					return value > 0 ? 1 : -1;
			}

			return value;
		}
		else
		{
			if ( value > 0 )
				return 1;
			if ( value < 0 )
				return -1;
		}

		return 0;
	}

	protected Vector2 ScreenPointToAnchoredPosition( Vector2 screenPosition )
	{
		if ( RectTransformUtility.ScreenPointToLocalPointInRectangle( _baseRect, screenPosition, _cam, out var localPoint ) )
		{
			var sizeDelta = _baseRect.sizeDelta;
			var pivotOffset = _baseRect.pivot * sizeDelta;
			return localPoint - background.anchorMax * sizeDelta + pivotOffset;
		}

		return Vector2.zero;
	}

	protected bool InZone( Vector2 screenPosition )
	{
		var scale = _canvasScalerRT.localScale;
		var canvasRTSize = _canvasScalerRT.rect.size;
		var scaledSizeX = canvasRTSize.x * scale.x;
		var scaledSizeY = canvasRTSize.y * scale.y;

		var xMinRaw = scaledSizeX * _paddingRelativeLeft;
		var xMaxRaw = scaledSizeX * (1f - _paddingRelativeRight);
		var xMin = Mathf.Min( xMinRaw, xMaxRaw );
		var xMax = Mathf.Max( xMinRaw, xMaxRaw );

		var yMinRaw = scaledSizeY * _paddingRelativeBottom;
		var yMaxRaw = scaledSizeY * (1f - _paddingRelativeTop);
		var yMin = Mathf.Min( yMinRaw, yMaxRaw );
		var yMax = Mathf.Max( yMinRaw, yMaxRaw );


		Vector2 rectPos = default;
		//RectTransformUtility.ScreenPointToLocalPointInRectangle( _canvasScalerRT, screenPosition, _cam, out rectPos );
		
		var inZone = screenPosition.x >= xMin && screenPosition.x <= xMax && 
					 screenPosition.y >= yMin && screenPosition.y <= yMax;

		//var inZone = rectPos.x >= xMin && rectPos.x <= xMax &&
		//			 rectPos.y >= yMin && rectPos.y <= yMax;

		/*Debug.Log( $"pos [{screenPosition.x},{screenPosition.y}], " +
				   $"canvasScaler {canvasRTSize.x}x{canvasRTSize.y}, rectPos [{rectPos.x},{rectPos.y}]" );*/

		return inZone;
	}
}

public enum AxisOptions
{
	Both,
	Horizontal,
	Vertical
}