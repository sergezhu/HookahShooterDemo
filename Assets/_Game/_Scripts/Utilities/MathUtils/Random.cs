namespace _Game._Scripts.Utilities.MathUtils
{
	using UnityEngine;

	public static class Random
	{
		public static float RandomRange( Vector2 range ) => UnityEngine.Random.Range( range.x, range.y );
		public static float RandomRange01() => UnityEngine.Random.Range( 0, 1f );
		public static float RandomPlusMinus1() => UnityEngine.Random.value * 2 - 1;
		public static Vector2 Vector2RandomPlusMinus1() => new Vector2( RandomPlusMinus1(), RandomPlusMinus1() );
		public static Vector3 Vector3RandomPlusMinus1() => new Vector3( RandomPlusMinus1(), RandomPlusMinus1(), RandomPlusMinus1() );
		public static Vector2 Vector2Random01() => new Vector2( RandomRange01(), RandomRange01() );
		public static Vector3 Vector3Random01() => new Vector3( RandomRange01(), RandomRange01(), RandomRange01() );
		public static bool RandomBool( float w0, float w1 ) => UnityEngine.Random.Range( 0, w0 + w1 ) > w0;
		public static bool RandomBool() => UnityEngine.Random.Range( 0, 2 ) != 0;
	}
}