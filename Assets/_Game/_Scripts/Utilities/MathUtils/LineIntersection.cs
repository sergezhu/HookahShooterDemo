namespace _Game._Scripts.Utilities.MathUtils
{
    using System;
    using UnityEngine;

    // https://forum.unity.com/threads/line-intersection.17384/
    
    public static class LineIntersection
    {
        public static bool HasIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, ref Vector2 intersection)
        {

            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num /*,offset*/;
            float x1lo, x1hi, y1lo, y1hi;

            Ax = p2.x - p1.x;
            Bx = q1.x - q2.x;

            // X bound box test/

            if (Ax < 0)
            {
                x1lo = p2.x;
                x1hi = p1.x;

            }else
            {
                x1hi = p2.x;
                x1lo = p1.x;
            }

            if (Bx > 0)
            {
                if (x1hi < q2.x || q1.x < x1lo) return false;
            }
            else
            {
                if (x1hi < q1.x || q2.x < x1lo) return false;

            }

            Ay = p2.y - p1.y;
            By = q1.y - q2.y;

            // Y bound box test//

            if (Ay < 0)
            {
                y1lo = p2.y;
                y1hi = p1.y;
            }
            else
            {
                y1hi = p2.y;
                y1lo = p1.y;
            }

            if (By > 0)
            {
                if (y1hi < q2.y || q1.y < y1lo) return false;
            }
            else
            {
                if (y1hi < q1.y || q2.y < y1lo) return false;
            }

            Cx = p1.x - q1.x;
            Cy = p1.y - q1.y;
            d = By * Cx - Bx * Cy; // alpha numerator//
            f = Ay * Bx - Ax * By; // both denominator//

            // alpha tests//

            if (f > 0)
            {
                if (d < 0 || d > f) return false;
            }
            else
            {
                if (d > 0 || d < f) return false;
            }
            
            e = Ax * Cy - Ay * Cx; // beta numerator//

            // beta tests //

            if (f > 0)
            {
                if (e < 0 || e > f) return false;
            }
            else
            {
                if (e > 0 || e < f) return false;
            }

            // check if they are parallel

            if (f == 0) return false;

            // compute intersection coordinates //

            num = d * Ax; // numerator //

//    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

//    intersection.x = p1.x + (num+offset) / f;
            intersection.x = p1.x + num / f;

            num = d * Ay;

//    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;

//    intersection.y = p1.y + (num+offset) / f;
            intersection.y = p1.y + num / f;

            return true;
        }

        // line1 defined via p1 and p2, line2 defined via p3 and p4
        public static bool HasFasterIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {

            Vector2 a = p2 - p1;
            Vector2 b = q1 - q2;
            Vector2 c = p1 - q1;

            float alphaNumerator = b.y * c.x - b.x * c.y;
            float alphaDenominator = a.y * b.x - a.x * b.y;
            float betaNumerator = a.x * c.y - a.y * c.x;
            float betaDenominator = a.y * b.x - a.x * b.y;

            bool doIntersect = true;

            if (alphaDenominator == 0 || betaDenominator == 0)
            {
                doIntersect = false;
            }
            else
            {

                if (alphaDenominator > 0)
                {
                    if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                    {
                        doIntersect = false;

                    }
                }
                else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
                {
                    doIntersect = false;
                }

                if (doIntersect && betaDenominator > 0) {
                    if (betaNumerator < 0 || betaNumerator > betaDenominator)
                    {
                        doIntersect = false;
                    }
                } else if (betaNumerator > 0 || betaNumerator < betaDenominator)
                {
                    doIntersect = false;
                }
            }

            return doIntersect;
        }


        // Intersection between straights (no segments)
        public static Vector2 GetIntersection( Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2 )
        {
            float k1 = K( p1, p2 );
            float k2 = K( q1, q2 );
            float b1 = B( p1, k1 );
            float b2 = B( q1, k2 );
            
            Vector2 intersection;

            if ( Math.Abs( k1 - k2 ) < float.Epsilon )
                intersection = new Vector2( p2.x, p2.y );
            else
            {
                float x = (b2 - b1) / (k1 - k2);
                float y = (k1 * x) + b1;

                intersection = new Vector2( x, y );
            }

            return intersection;
        }

        public static Vector2? GetIntersectionOnSegment( Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2 )
        {
            Vector2 intersection = GetIntersection( p1, p2, q1, q2 );
            Vector2? result = null;

            if ( IsOnSegment( p1, p2, intersection ) ) 
                result = intersection;

            return result;

        }

        private static float K( Vector2 p1, Vector2 p2 )
        {
            return (p2.y - p1.y) / (p2.x - p1.x);
        }

        private static float B( Vector2 p1, float k )
        {
            return p1.y - (k * p1.x);
        }

        private static bool IsOnSegment( Vector2 a, Vector2 b, Vector2 p )
        {
            var epsilon = 0.001f;
            
            var sqrPA = (p.x - a.x) * (p.x - a.x) + (p.y - a.y) * (p.y - a.y);
            var sqrPB = (p.x - b.x) * (p.x - b.x) + (p.y - b.y) * (p.y - b.y);

            return Mathf.Abs( sqrPA - sqrPB ) < epsilon;
        }
    }
}