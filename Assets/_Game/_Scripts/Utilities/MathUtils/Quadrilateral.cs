namespace _Game._Scripts.Utilities.MathUtils
{
	using System;
	using System.Linq;
	using _Game._Scripts.Utilities.Extensions;
	using UnityEngine;

	public class Quadrilateral
	{
		private const int  ThrowPointsSteps = 100;
		private const float  InsetFactor = 0.75f;
		
		public Quadrilateral( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4 )
		{
			_cachedPoints = new [] {p1, p2, p3, p4};
		}
		
		public Quadrilateral( Vector2[] points )
		{
			_cachedPoints = new [] { points[0], points[1], points[2], points[3] };
		}
		public Quadrilateral()
		{
			_cachedPoints = new[] {Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};
		}

		private readonly Vector2[] _cachedPoints;

		public Vector2 P1
		{
			get => _cachedPoints[0];
			set => _cachedPoints[0] = value;
		}

		public Vector2 P2
		{
			get => _cachedPoints[1];
			set => _cachedPoints[1] = value;
		}

		public Vector2 P3
		{
			get => _cachedPoints[2];
			set => _cachedPoints[2] = value;
		}

		public Vector2 P4
		{
			get => _cachedPoints[3];
			set => _cachedPoints[3] = value;
		}

		// Points must be pass sequentially ccw or cw
		public Vector2 GetCenter()
		{
			return (P1 + P2 + P3 + P4) * 0.25f;
		}

		public Quadrilateral GetInset( float insetFactor )
		{
			insetFactor = Mathf.Clamp01( insetFactor );

			var result = new Quadrilateral();
			var center = GetCenter();

			result.P1 = center + insetFactor * (P1 - center);
			result.P2 = center + insetFactor * (P2 - center);
			result.P3 = center + insetFactor * (P3 - center);
			result.P4 = center + insetFactor * (P4 - center);

			return result;
		}

		/*public (Quadrilateral, Quadrilateral) Slice()
		{
			var insetQL = GetInset( InsetFactor );
			var point = insetQL.TakeRandomPoint();
			var rndDir = Utils.Vector2RandomPlusMinus1().normalized;

			var q1 = point;
			var q2 = point + rndDir;

			for ( int i = 0; i < 4; i++ )
			{
				var cur = i;
				var next = i == 3 ? 0 : i + 1;
				var pCur = _cachedPoints[cur];
				var pNext = _cachedPoints[next];

				var intersectPoint = LineIntersection.GetIntersectionOnSegment( pCur, pNext, q1, q2 );
			}		
			
		}*/

		public Rect GetBound()
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;

			for ( int i = 0; i < 4; i++ )
			{
				var p = _cachedPoints[i];

				if ( p.x < minX )
					minX = p.x;

				if ( p.y < minY )
					minY = p.y;

				if ( p.x > maxX )
					maxX = p.x;

				if ( p.y > maxY )
					maxY = p.y;
			}

			return new Rect( minX, minY, maxX - minX, maxY - minY );
		}

		public Vector2 TakeRandomPoint()
		{
			for ( int i = 0; i < ThrowPointsSteps; i++ )
			{
				Vector2 p = TakeRandomPointInBounds();
				var contains = Contains( p );

				if ( contains )
					return p;
			}

			throw new InvalidOperationException( "You are could not take random point" );
		}

		public bool Contains(Vector2 p)
		{
			var isInsideInTris1 = p.InTriangle( P1, P2, P3 );
			var isInsideInTris2 = p.InTriangle( P3, P4, P1 );

			return isInsideInTris1 || isInsideInTris2;
		}

		public bool IsOverlappedWith( Quadrilateral other )
		{
			var otherPoints = new Vector2[] { other.P1, other.P2, other.P2, other.P4 };

			bool contains12 = otherPoints.Any( p => Contains( p ) );
			bool contains21 = _cachedPoints.Any( p => other.Contains( p ) );
			
			return contains21 || contains12;
		}

		private Vector2 TakeRandomPointInBounds()
		{
			return GetBound().RandomPoint();
		}
	}
}