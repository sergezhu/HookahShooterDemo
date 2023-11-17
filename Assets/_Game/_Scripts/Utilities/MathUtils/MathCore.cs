namespace _Game._Scripts.Utilities.MathUtils
{
	using UnityEngine;

	public static class MathCore
	{
		public static float Epsilon => float.Epsilon;
		public static float Abs( float v ) => Mathf.Abs( v );
		public static int Abs( int v ) => Mathf.Abs( v );
		public static float Sign( float v ) => Abs( v ) < Epsilon ? 0 : v > 0 ? 1f : -1f;
		public static int Sign( int v ) => v == 0 ? 0 : v > 0 ? 1 : -1;
	}
}