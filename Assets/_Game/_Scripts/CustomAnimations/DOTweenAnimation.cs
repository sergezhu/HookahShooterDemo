using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts.CustomAnimations
{
    public abstract class DOTweenAnimation : MonoBehaviour
    {
        public Sequence Sequence { get; protected set; }

        protected void TryKillAndCreateNewSequence()
        {
            TryKillSequence();

            Sequence = DOTween.Sequence();
        }

        protected void TryKillSequence()
        {
            if (Sequence == null) return;

            Sequence.Kill();
        }

        protected void TryLoopSequence(AnimationLoopProperties properties)
        {
            if (Sequence == null) return;

            Sequence.SetLoops(properties.Loops, properties.Type);
        }
    }
}