using UnityEngine;
using Zenject;

namespace _Game._Scripts.Installers
{
    using _Game._Scripts.Persistent;

    public class PersistentInstaller : MonoInstaller
    {
        [SerializeField] private PersistentBootstrap _persistentBootstrap;
    
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PersistentBootstrap>().FromInstance( _persistentBootstrap );
        }
    }
}