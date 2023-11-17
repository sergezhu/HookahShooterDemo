namespace _Game._Scripts.Level
{
	using _Game._Scripts.Interfaces;
	using UnityEngine;
	using Zenject;

	public class TargetColliderTag : MonoBehaviour
	{
		private ITarget _target;

		public ITarget Target => _target;

		[Inject]
		public void Construct( ITarget target )
		{
			_target = target;
		} 
	}
}