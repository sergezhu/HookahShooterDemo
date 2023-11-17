namespace _Game._Scripts.UI
{
	using _Game._Scripts.UI.Base;
	using UniRx;
	using UnityEngine.EventSystems;

	public class UIZone : UIBaseButton, IPointerEnterHandler, IPointerExitHandler
	{
		public ReactiveProperty<bool> InZone { get; } = new ReactiveProperty<bool>();

		public void OnPointerEnter( PointerEventData eventData )
		{
			//Debug.Log( $"{name} Enter" );
			InZone.Value = true;
		}

		public void OnPointerExit( PointerEventData eventData )
		{
			//Debug.Log( $"{name} Exit" );
			InZone.Value = false; 
		}
	}
}