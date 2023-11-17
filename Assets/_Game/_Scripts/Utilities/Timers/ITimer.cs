namespace _Game._Scripts.Utilities.Timers
{
	using System;

	public interface ITimer
	{
		public void Start(Action onCompleteCallback = null);
		public void Complete(bool canInvokeCallback);
		public void Tick( double deltaTime );
	}
}