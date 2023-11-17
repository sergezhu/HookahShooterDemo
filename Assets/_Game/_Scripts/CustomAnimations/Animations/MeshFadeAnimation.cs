namespace _Game._Scripts.CustomAnimations.Animations
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    public class MeshFadeAnimation : CustomAnimation
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] [Range(0, 1)] float _originFade;
        [SerializeField] [Range(0, 1)] float _targetFade;

        public float OriginFade
        {
            get => _originFade;
            set => _originFade = value;
        }

        public float TargetFade
        {
            get => _targetFade;
            set => _targetFade = value;
        }

        public void SetFade( float value )
        {
            var color = _meshRenderer.sharedMaterial.color;
            color.a = value;
            _meshRenderer.sharedMaterial.color = color;
        }

        public override void Play(Action callback = null)
        {
            TryKillAndCreateNewSequence();

            SetFade( _originFade );

            Sequence.Append(
                DOVirtual.Float( _originFade, _targetFade, Properties.Duration, SetFade )
                    .SetEase(Properties.Ease, Properties.EaseOvershoot));

            PostProcessAnimation(callback);
        }

        public override void Stop()
        {
            TryKillSequence();
        }
    }
}
