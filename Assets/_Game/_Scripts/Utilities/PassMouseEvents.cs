namespace _Game._Scripts.Utilities
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	public class PassMouseEvents : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private bool _reversePass;
		[SerializeField] private GameObject _currentPointerTarget = null;
		[SerializeField] private GameObject _onDownPointerTarget = null;

		private GraphicRaycaster _raycaster;
		private List<RaycastResult> _hits;
		private List<GameObject> _onEnterAndExitIgnoredObjects;

		private GraphicRaycaster Raycaster => _raycaster ??= GetComponentInParent<GraphicRaycaster>();

		public void SetOnEnterAndExitIgnoredObjects( IEnumerable<GameObject> objects )
		{
			_onEnterAndExitIgnoredObjects = objects.ToList();
		}


		private void Awake()
		{
			_hits = new List<RaycastResult>( 30 );
		}

		private void Update()
		{
			if ( Raycaster == null )
			{
				Debug.LogWarning( "Raycaster not found!" );
				return;
			}

			var eventData = new PointerEventData( EventSystem.current ) { position = Input.mousePosition };
			var hitObject = ThrowRaycast( eventData, ref _hits, _onEnterAndExitIgnoredObjects );
			GameObject target;

			#if UNITY_EDITOR
			target = hitObject;
			#else
			var isTouched = Input.touchCount > 0;
			target = isTouched ? hitObject : null;
			#endif
			

			if ( target != _currentPointerTarget )
			{
				if ( _currentPointerTarget != null )
				{
					//Debug.Log( $"exit : {_currentPointerTarget.name}" );
					_currentPointerTarget.SendMessage( "OnPointerExit", eventData, SendMessageOptions.DontRequireReceiver );
				}

				if ( target != null )
				{
					//Debug.Log( $"enter : {target.name}" );
					target.SendMessage( "OnPointerEnter", eventData, SendMessageOptions.DontRequireReceiver );
				}

				_currentPointerTarget = target;
			}

			//var targetName = _currentPointerTarget == null ? string.Empty : _currentPointerTarget.name;
			//Debug.Log( $"Pointer over : {targetName}" );
		}

		void IPointerClickHandler.OnPointerClick( PointerEventData eventData )
		{
			/*var targetName = _currentPointerTarget == null ? string.Empty : $"{_currentPointerTarget.name}";
			var onDownPointerName = _onDownPointerTarget == null ? string.Empty : $"{_onDownPointerTarget.name}";
			
			Debug.Log( $"OnPointerClick -> Pointer target : [{targetName}], OnDownPointerTarget : [{onDownPointerName}]" );*/

			var target = ThrowRaycast( eventData, ref _hits );
			
			if ( target != null && target == _onDownPointerTarget)
				target.SendMessage( "OnPointerClick", eventData, SendMessageOptions.DontRequireReceiver );
		}

		void IPointerDownHandler.OnPointerDown( PointerEventData eventData )
		{
			/*var targetName = _currentPointerTarget == null ? string.Empty : $"{_currentPointerTarget.name}";
			var onDownPointerName = _onDownPointerTarget == null ? string.Empty : $"{_onDownPointerTarget.name}";

			Debug.Log( $"OnPointerDown -> Pointer target : [{targetName}], OnDownPointerTarget : [{onDownPointerName}]" );*/
			
			/*if ( _currentPointerTarget != null )
			{
				_onDownPointerTarget = _currentPointerTarget;
				_currentPointerTarget.SendMessage( "OnPointerDown", eventData, SendMessageOptions.DontRequireReceiver );
			}*/

			var target = ThrowRaycast( eventData, ref _hits );

			if ( target != null )
			{
				_onDownPointerTarget = target;
				target.SendMessage( "OnPointerDown", eventData, SendMessageOptions.DontRequireReceiver );
			}
		}

		void IPointerUpHandler.OnPointerUp( PointerEventData eventData )
		{
			if ( _onDownPointerTarget != null )
				_onDownPointerTarget.SendMessage( "OnPointerUp", eventData, SendMessageOptions.DontRequireReceiver );
		}


		private GameObject ThrowRaycast( PointerEventData eventData, ref List<RaycastResult> hits, List<GameObject> ignoredObjects = null )
		{
			GameObject target = null;
			
			hits.Clear();
			Raycaster.Raycast( eventData, hits );


			if ( _reversePass )
			{
				for ( int i = hits.Count - 1; i >= 0; i-- )
				{
					if ( hits[i].gameObject == gameObject )
						continue;
					
					if( ignoredObjects!= null && ignoredObjects.Contains( hits[i].gameObject ))
						continue;

					target = hits[i].gameObject;
					break;
				}
			}
			else
			{
				for ( int i = 0; i <= hits.Count - 1; i++ )
				{
					if ( hits[i].gameObject == gameObject )
						continue;

					if ( ignoredObjects != null && ignoredObjects.Contains( hits[i].gameObject ) )
						continue;

					target = hits[i].gameObject;
					break;
				}
			}

			return target;
		}
	}
}