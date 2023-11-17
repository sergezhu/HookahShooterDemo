namespace _Game._Scripts.Character.Animations
{
	using System.Collections;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Character.Unit;
	using UnityEngine;
	using Zenject;

	public interface IAnimatorController
	{
		
		void DoMeleeAnimation();
		void DoShootAnimation();
	}

	public class UnitAnimatorController : MonoBehaviour, IAnimatorController
	{
		[SerializeField] private Animator _animator;

		[Space]
		[SerializeField] private float _speedTransitionMultiplier = .5f;
		[SerializeField] private float _normalMoveAnimationMultiplier = .7f;
		[SerializeField] private float _aimMoveAnimationMultiplier = 1.5f;
		[SerializeField] private float _animatorSmooth = 0.95f;

		private IUnitView _view;
		private AnimatorControllerLayer _fullMovementLayer;

		private float _currentForwardValue;
		private float _currentCommonValue;

		[Inject]
		private void Construct( IUnitView view )
		{
			_view = view;
		}

		private void Start()
		{
			_fullMovementLayer = new AnimatorControllerLayer( _animator, 1, "MovementFull" );
			_animator.SetFloat( AnimatorHashes.NormalMoveAnimationMultiplier, 0.75f );
			
			SetVelocity( Vector3.zero );
		}

		public void SetVelocity( Vector3 velocity )
		{
			if ( _animator == null )
				return;

			Vector3 dir = velocity.normalized;
			float magnitude = velocity.magnitude;

			if ( float.IsNaN( magnitude ) )
			{
				Debug.LogError( $"magnitude is NaN : dir [{dir.x} {dir.y} {dir.z}], vel [{velocity.x} {velocity.y} {velocity.z}], magnitude [{magnitude}], test : {(Vector3.zero).magnitude}" );
			}

			var forwardValue = magnitude > 0.01f
				? _speedTransitionMultiplier * magnitude * Vector3.Dot( dir, _view.Forward )
				: 0;

			var sideValue = magnitude > 0.01f
				? _speedTransitionMultiplier * magnitude * Vector3.Dot( dir, _view.Right )
				: 0;

			var commonValue = _speedTransitionMultiplier * magnitude;

			_currentForwardValue = Mathf.Lerp( _currentForwardValue, forwardValue, _animatorSmooth );
			_currentCommonValue = Mathf.Lerp( _currentCommonValue, commonValue, _animatorSmooth );

			_animator.SetFloat( AnimatorHashes.ForwardVelocity, _currentForwardValue );
			_animator.SetFloat( AnimatorHashes.RightVelocity, sideValue );
			_animator.SetFloat( AnimatorHashes.CommonVelocity, _currentCommonValue );
		}

		public void SetForwardSpeed( float speed )
		{
			var velocity = speed * _view.Forward;
			SetVelocity( velocity );
		}

		public void DoMeleeAnimation()
		{
			_animator.SetTrigger( AnimatorHashes.MeleeAttackTrigger );
		}

		public void DoShootAnimation()
		{
			_animator.SetTrigger( AnimatorHashes.ShootTrigger );
		}

		public void HandleDead()
		{
			_animator.SetTrigger( AnimatorHashes.DeadTrigger );
		}

		public void CleanUp()
		{
			var layers = new[] { _fullMovementLayer };

			foreach ( var layer in layers )
			{
				layer.CleanUp();
			}
		}
	}
}