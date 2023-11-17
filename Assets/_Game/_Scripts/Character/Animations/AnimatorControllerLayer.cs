namespace _Game._Scripts.Character.Animations
{
	using System;
	using DG.Tweening;
	using UnityEngine;

	public class AnimatorControllerLayer
	{
		private Tween _weightTween;
		private readonly Animator _animator;
		private float _currentLayerWeight;
		private readonly string _layerName;
		private int _layerIndex;

		public AnimatorControllerLayer( Animator animator, float startLayerWeight, string layerName )
		{
			Debug.Log( $"layer name = {layerName}" );
		
			
			_animator = animator;
			_currentLayerWeight = startLayerWeight;
			_layerName = layerName;
			_layerIndex = _animator.GetLayerIndex( layerName );

			//Debug.Log( $"layer name = {_layerName}, layer index = {_layerIndex}" );
			 
			SetWeight( _currentLayerWeight );
		}

		public float GetWeight() => _currentLayerWeight;

		public void SetWeight(float weight)
		{
			_currentLayerWeight = Mathf.Clamp( weight, 0, 1f );
			_animator.SetLayerWeight( _layerIndex, _currentLayerWeight );
		}

		public void TweenWeightTo( float targetWeight, float duration, Action onComplete )
		{
			_weightTween?.Kill();
			_weightTween = DOVirtual.Float( _currentLayerWeight, targetWeight, duration, value =>
				{
					_currentLayerWeight = value;
					_animator.SetLayerWeight( _layerIndex, _currentLayerWeight );
				} )
				.OnComplete( () =>
				{
					_weightTween = null;
					onComplete?.Invoke();
				} );
		}

		public void CleanUp()
		{
			_weightTween?.Kill();
		}
	}
}