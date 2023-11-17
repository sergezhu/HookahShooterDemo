namespace _Game._Scripts.Character.Hero
{
    using _Game._Scripts.Character.Unit;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class HeroAnimationEventsProvider : MonoBehaviour, IInitializable, IAttackEventsProvider
    {
        public ReactiveCommand MeleeHit { get; } = new ReactiveCommand();
        public ReactiveCommand RangeFire { get; } = new ReactiveCommand();

        public void Initialize()
        {
        }

        public void OnHit()
        {
            MeleeHit.Execute();
        }

        public void OnFire()
        {
            RangeFire.Execute();
        }
    }
}
