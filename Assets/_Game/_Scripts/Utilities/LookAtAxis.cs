namespace _Game._Scripts.Utilities
{
    using UnityEngine;

    [ExecuteAlways]
    public class LookAtAxis : MonoBehaviour
    {
        [SerializeField] private Vector3 _worldAxis;

        [ExecuteAlways]
        private void LateUpdate()
        {
            //transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.forward, Vector3.up);
            transform.forward = _worldAxis;
        }
    }
}