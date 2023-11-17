using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Game._Scripts.Navigation
{
    public class CustomNavMeshAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
    
        [SerializeField, Space] private float _stopDistance;
        [SerializeField] private LayerMask _obstacleLayers;
        [SerializeField] private float _samplePositionDistance = 10;
    
        [field:SerializeField, ReadOnly] public int CurrentCornerIndex { get; private set; }

        public float StopDistance => _stopDistance;
        public NavMeshPath Path { get; private set; }
        public Vector3 CurrentPathCorner => PathCheck(CurrentCornerIndex) ? Path.corners[CurrentCornerIndex] : Vector3.zero;
        public Vector3 Destination => IsPathExist ? Path.corners[Path.corners.Length - 1] : Vector3.zero;
        public bool IsPathExist => Path != null;

        public bool PathCheck(int index) => IsHasCorners(index) & IsPathExist;
        public bool IsHasCorners(int index) => index < Path.corners.Length;
        public bool IsPathCompleted => IsPathExist && Path.status == NavMeshPathStatus.PathComplete;
        public LayerMask ObstacleLayers => _obstacleLayers;

        public void PavePathToRandomPoint(Vector3 sourcePosition, float radius)
        {
            Vector3 randomPoint = FindRandomPoint(sourcePosition, radius);
            PavePathToPoint(randomPoint);
        }

        public void PavePathToRandomPointWithoutObstacle(Vector3 sourcePosition, float radius)
        {
            Vector3 randomPointWithoutObstacle = FindRandomPointWithoutObstacle(sourcePosition, radius);
            PavePathToPoint(randomPointWithoutObstacle);
        }

        public Vector3 FindRandomPoint(Vector3 sourcePosition, float radius)
        {
            Vector3 randomDirection = sourcePosition + (Random.insideUnitSphere * radius);
            NavMesh.SamplePosition(randomDirection, out NavMeshHit navMeshHit, radius, NavMesh.AllAreas);
            return navMeshHit.position;
        }

        public Vector3 FindRandomPointWithoutObstacle(Vector3 sourcePosition, float radius)
        {
            Vector3 randomPosition = Vector3.zero;
            bool isValidPositionFinded = false;
            while (isValidPositionFinded == false)
            {
                randomPosition = FindRandomPoint(sourcePosition, radius);

                if (randomPosition.y < -10000 || randomPosition.y > 10000) continue;

                if (Physics.Linecast(sourcePosition + Vector3.up, randomPosition, out RaycastHit navMeshHit, _obstacleLayers) == false)
                {
                    isValidPositionFinded = true;
                    Debug.DrawLine(randomPosition, randomPosition + Vector3.up, Color.green, 10f);
                    Debug.DrawLine(sourcePosition + Vector3.up * 0.2f, randomPosition, Color.yellow, 10f);
                }
            }

            return randomPosition;
        }

        public void PavePathToPoint(Vector3 targetPoint)
        {
            Path = new NavMeshPath();
            /*NavMesh.SamplePosition(targetPoint, out NavMeshHit hit, _samplePositionDistance, NavMesh.AllAreas);
        NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, Path);*/

            _agent.CalculatePath( targetPoint, Path );

            CurrentCornerIndex = 1;

            if(Path.status == NavMeshPathStatus.PathInvalid)
            {
                Path = null;
                Debug.LogWarning("Path is not paved");
            }
        }

        public void ResetPath()
        {
            Path = null;
            CurrentCornerIndex = 0;
        }

        public bool TryIncreaseCurrentCornerIndex()
        {
            if (IsPathExist == false) 
                return false;

            //Debug.Log( $"TryIncreaseCurrentCornerIndex : current [{CurrentCornerIndex}], last [{Path.corners.Length - 1}]" );

            if (CurrentCornerIndex < Path.corners.Length - 1)
            {
                CurrentCornerIndex++;
                return true;
            }

            return false;
        }

        public bool IsPointAchievable(Vector3 point)
        {
            NavMeshPath testPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, testPath);
            return testPath.status == NavMeshPathStatus.PathComplete;
        }

        private void OnDrawGizmos()
        {
            if (Path == null) return;

            for (int i = 0; i < Path.corners.Length; i++)
            {
                Gizmos.color = i == CurrentCornerIndex ? Color.green : Color.red;
                Gizmos.DrawSphere(Path.corners[i], 0.15f);

                if (i < Path.corners.Length - 1)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(Path.corners[i], Path.corners[i + 1]);
                }
            }
        }

        private void OnValidate()
        {
            TryGetComponent<NavMeshAgent>( out _agent );
        }
    }
}