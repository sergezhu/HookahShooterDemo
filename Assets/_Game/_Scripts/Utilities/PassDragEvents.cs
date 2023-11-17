namespace _Game._Scripts.Utilities
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	public class PassDragEvents : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField] private bool _reversePass;
		[SerializeField] private GameObject _draggingProxyTarget = null;

		private GraphicRaycaster _raycaster;
		
		private GraphicRaycaster Raycaster => _raycaster ??= GetComponentInParent<GraphicRaycaster>();

		void IBeginDragHandler.OnBeginDrag( PointerEventData eventData )
		{
			_draggingProxyTarget = null;

			if ( Raycaster == null )
			{
				Debug.LogWarning( "Raycaster not found!" );
				return;
			}

			_draggingProxyTarget = ThrowRaycast( eventData, Raycaster, gameObject, _reversePass );

			if ( _draggingProxyTarget != null )
			{
				_draggingProxyTarget.SendMessage( "OnBeginDrag", eventData, SendMessageOptions.DontRequireReceiver );
			}
		}

		void IDragHandler.OnDrag( PointerEventData eventData )
		{
			if ( _draggingProxyTarget != null )
				_draggingProxyTarget.SendMessage( "OnDrag", eventData, SendMessageOptions.DontRequireReceiver );
		}

		void IEndDragHandler.OnEndDrag( PointerEventData eventData )
		{
			if ( _draggingProxyTarget != null )
			{
				_draggingProxyTarget.SendMessage( "OnEndDrag", eventData, SendMessageOptions.DontRequireReceiver );
				_draggingProxyTarget = null;
			}
		}


		public static GameObject ThrowRaycast( PointerEventData eventData, GraphicRaycaster raycaster, GameObject self, bool reversePass )
		{
			GameObject target = null;
			
			List<RaycastResult> hits = new List<RaycastResult>();
			raycaster.Raycast( eventData, hits );


			if ( reversePass )
			{
				for ( int i = hits.Count - 1; i >= 0; i-- )
				{
					if ( hits[i].gameObject == self )
						continue;

					target = hits[i].gameObject;
					break;
				}
			}
			else
			{
				for ( int i = 0; i <= hits.Count - 1; i++ )
				{
					if ( hits[i].gameObject == self )
						continue;

					target = hits[i].gameObject;
					break;
				}
			}

			return target;
		}
	}
}