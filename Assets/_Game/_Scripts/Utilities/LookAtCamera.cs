namespace _Game._Scripts.Utilities
{
    using UnityEngine;

    [ExecuteAlways]
    public class LookAtCamera : MonoBehaviour
    {
        private Camera _camera;
        private Transform _cameraTransform;

        private Camera Camera => _camera ??= Camera.main;
        private Transform CameraTransform => _cameraTransform ??= Camera.transform;


        [ExecuteAlways]
        private void LateUpdate()
        {
            //transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.forward, Vector3.up);
            transform.rotation = Quaternion.LookRotation( transform.position - CameraTransform.position, CameraTransform.transform.up );
        }
    }
}