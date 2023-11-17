namespace _Game._Scripts.Utilities.Timers
{
	using System;
	using UnityEngine;

	public enum TimerState
	{
		Ready,
		Running,
		Completed
	}

	public class SimpleTimer : ITimer
	{
		private Action _onCompleteCallback;
		private float _duration;
		private double _value;

		public TimerState State { get; private set; }

		public SimpleTimer( float duration )
		{
			_duration = duration;
			_value = 0;

			State = TimerState.Ready;
		}

		public void Start( Action onCompleteCallback = null )
		{
			_onCompleteCallback = onCompleteCallback;
			_value = _duration;

			State = TimerState.Running;

			//Debug.Log( $"Timer start {Time.time}" );
		}

		public void Reset()
		{
			_onCompleteCallback = null;
			_value = 0;

			State = TimerState.Ready;

			//Debug.Log( $"Timer reset {Time.time}" );
		}

		public void Complete( bool canInvokeCallback )
		{
			CompleteInternal( canInvokeCallback );
		}

		public void ChangeDuration( float newDuration, bool needScaleCurrentProgress )
		{
			if ( needScaleCurrentProgress )
			{
				_value *= newDuration / _duration;
			}
			else
			{
				var delta = newDuration - _duration;
				_value += delta;
			}

			_duration = newDuration;
		}

		public void Tick( double deltaTime )
		{
			if ( State != TimerState.Running )
				return;

			//Debug.Log( $"[Timer] State : {State} : {_value}" );

			_value -= deltaTime;

			if ( _value <= 0 )
			{
				CompleteInternal( true );
			}
		}

		private void CompleteInternal( bool canInvokeCallback )
		{
			_value = 0;

			State = TimerState.Completed;
			//Debug.Log( $"[Timer] State : {State} : {_value}" );

			if ( canInvokeCallback )
			{
				_onCompleteCallback?.Invoke();
			}
		}
	}
}