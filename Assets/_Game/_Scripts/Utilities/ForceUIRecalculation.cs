namespace _Game._Scripts.Utilities
{
	using System;
	using UnityEngine;

	[ExecuteAlways]
	public class ForceUIRecalculation : MonoBehaviour
	{
		[SerializeField] private Vector2 _anchoredCoords;
		
		private RectTransform _rectTransform;

		private RectTransform RT => _rectTransform ? _rectTransform : GetComponent<RectTransform>();

		[ExecuteAlways]
		private void Update()
		{
			RT.anchoredPosition = _anchoredCoords;
			RT.ForceUpdateRectTransforms();
		}
	}
}