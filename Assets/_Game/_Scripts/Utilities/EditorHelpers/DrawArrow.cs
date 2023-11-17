namespace _Game._Scripts.Utilities.EditorHelpers
{
    using UnityEngine;

    public static class DrawArrow
    {
        public static void ForGizmo( Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
        {
            Gizmos.DrawRay( pos, direction );

            Vector3 right = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 + arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Vector3 left = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 - arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Gizmos.DrawRay( pos + direction, right * arrowHeadLength );
            Gizmos.DrawRay( pos + direction, left * arrowHeadLength );
        }

        public static void ForGizmo( Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
        {
            Gizmos.color = color;
            Gizmos.DrawRay( pos, direction );

            Vector3 right = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 + arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Vector3 left = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 - arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Gizmos.DrawRay( pos + direction, right * arrowHeadLength );
            Gizmos.DrawRay( pos + direction, left * arrowHeadLength );
        }

        public static void ForDebug( Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
        {
            Debug.DrawRay( pos, direction );

            Vector3 right = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 + arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Vector3 left = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 - arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Debug.DrawRay( pos + direction, right * arrowHeadLength );
            Debug.DrawRay( pos + direction, left * arrowHeadLength );
        }
        public static void ForDebug( Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
        {
            Debug.DrawRay( pos, direction, color );

            Vector3 right = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 + arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Vector3 left = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 - arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Debug.DrawRay( pos + direction, right * arrowHeadLength, color );
            Debug.DrawRay( pos + direction, left * arrowHeadLength, color );
        }
        
        public static void ForGizmoV2(Vector3 a, Vector3 b, float arrowheadAngle, float arrowheadDistance, float arrowheadLength)
        {
            // Get the Direction of the Vector
            Vector3 dir = b - a;
 
            // Get the Position of the Arrowhead along the length of the line.
            Vector3 arrowPos = a + (dir * arrowheadDistance);
 
            // Get the Arrowhead Lines using the direction from earlier multiplied by a vector representing half of the full angle of the arrowhead (y)
            // and -1 for going backwards instead of forwards (z), which is then multiplied by the desired length of the arrowhead lines coming from the point.
 
            if(dir.magnitude <= float.Epsilon)
                return;
            
            Vector3 up = Quaternion.LookRotation(dir) * new Vector3(0f, Mathf.Sin(arrowheadAngle / 72), -1f) * arrowheadLength;
            Vector3 down = Quaternion.LookRotation(dir) * new Vector3(0f, -Mathf.Sin(arrowheadAngle / 72), -1f) * arrowheadLength;
            Vector3 left= Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin(arrowheadAngle / 72), 0f, -1f) * arrowheadLength;
            Vector3 right = Quaternion.LookRotation(dir) * new Vector3(-Mathf.Sin(arrowheadAngle / 72), 0f, -1f) * arrowheadLength;
 
            // Get the End Locations of all points for connecting arrowhead lines.
            Vector3 upPos = arrowPos + up;
            Vector3 downPos = arrowPos + down;
            Vector3 leftPos = arrowPos + left;
            Vector3 rightPos = arrowPos + right;
 
            // Draw the line from A to B
            Gizmos.DrawLine(a, b);
 
            // Draw the rays representing the arrowhead.
            Gizmos.DrawRay(arrowPos, up);
            Gizmos.DrawRay(arrowPos, down);
            Gizmos.DrawRay(arrowPos, left);
            Gizmos.DrawRay(arrowPos, right);
 
            // Draw Connections between rays representing the arrowhead
            Gizmos.DrawLine(upPos, leftPos);
            Gizmos.DrawLine(leftPos, downPos);
            Gizmos.DrawLine(downPos, rightPos);
            Gizmos.DrawLine(rightPos, upPos);
 
        }
    }
}
