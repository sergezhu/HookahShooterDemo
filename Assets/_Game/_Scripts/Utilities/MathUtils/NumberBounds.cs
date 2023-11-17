namespace _Game._Scripts.Utilities.MathUtils
{
	using System;
	using UnityEngine;

	public interface INumberBounds<out T> where T : struct
	{
		public T Min { get; }
		public T Max { get; }
	}
	
	[Serializable]
	public struct IntBounds : INumberBounds<int>
	{
		[SerializeField] private int _min;
		[SerializeField] private int _max;

		public IntBounds( int min, int max )
		{
			_min = min;
			_max = max;
		}
		public int Min => _min;
		public int Max => _max;
		
		public float GetRandom(bool includeMax)
		{
			var d = includeMax ? 1 : 0;
			return UnityEngine.Random.Range( _min, _max + d );
		}
	}
	
	[Serializable]
	public struct FloatBounds : INumberBounds<float>
	{
		[SerializeField] private float _min;
		[SerializeField] private float _max;

		public FloatBounds( float min, float max )
		{
			_min = min;
			_max = max;
		}
		public float Min => _min;
		public float Max => _max;
		public float GetRandom()
		{
			return UnityEngine.Random.Range( _min, _max );
		}
	}
}
