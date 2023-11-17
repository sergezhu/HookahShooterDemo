using UnityEngine;

namespace _Game._Scripts.Helpers.Mono
{
    public class TargetFrameRate : MonoBehaviour
    {
        [SerializeField] private int _targetFrameRate;

        private void Start()
        {
            Debug.Log($"Framerate before: {Application.targetFrameRate}"  );
        
            Application.targetFrameRate = _targetFrameRate;

            Debug.Log( $"Framerate after: {Application.targetFrameRate}" );
        }
    }
}
