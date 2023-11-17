using UnityEngine;

namespace _Game._Scripts.UI
{
    using _Game._Scripts.CustomAnimations.Animations;

    public class UILoadScreen : MonoBehaviour
    {
        [SerializeField] private FadeAnimation _fadeIn;
        [SerializeField] private FadeAnimation _fadeOut;

        public void FadeIn()
        {
            _fadeIn.Play();
            Debug.Log("Fade In");
        }

        public void FadeOut()
        {
            _fadeOut.Play();
            Debug.Log( "Fade Out" );
        }
    }
}
