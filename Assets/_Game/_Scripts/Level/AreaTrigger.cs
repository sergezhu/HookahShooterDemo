using UniRx;
using UnityEngine;

namespace _Game._Scripts.Level
{
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.Interfaces;

    [RequireComponent(typeof(Collider))]
    public class AreaTrigger : MonoBehaviour
    {
        private IHeroFacade _heroFacade;
    
        public ReactiveCommand<ITarget> Enter { get; } = new ReactiveCommand<ITarget>();
        public ReactiveCommand<ITarget> Exit { get; } = new ReactiveCommand<ITarget>();
    
        public ReadOnlyReactiveProperty<bool> IsInside { get; private set; }
        private ReactiveProperty<bool> IsInsideInternal { get; } = new ReactiveProperty<bool>();


        void OnTriggerEnter(Collider other) => HandleTriggerEvent(other, true);
        void OnTriggerExit(Collider other) => HandleTriggerEvent(other, false);

        void Awake()
        {
            IsInside = IsInsideInternal.ToReadOnlyReactiveProperty();
        
            var coll = GetComponent<Collider>();
            coll.isTrigger = true;
        }

        private void OnDisable()
        {
            if ( _heroFacade != null )
            {
                IsInsideInternal.Value = false;
                Exit.Execute( _heroFacade );

                _heroFacade = null;
            }
        }

        void HandleTriggerEvent(Collider other, bool isEnter)
        {
            if (other.TryGetComponent<TargetColliderTag>(out var targetColliderTag))
            {
                if ( targetColliderTag.Target is IHeroFacade hf)
                {
                    if ( isEnter )
                    {
                        _heroFacade = hf;
                        IsInsideInternal.Value = true;
                        Enter.Execute( targetColliderTag.Target );
                    }
                    else
                    {
                        _heroFacade = null;
                        IsInsideInternal.Value = false;
                        Exit.Execute( targetColliderTag.Target );
                    }
                }
            }
        }
    }
}