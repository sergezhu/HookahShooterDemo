using _Game._Scripts.UI.Hud;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Installers
{
    using _Game._Scripts.UI;
    using _Game._Scripts.UI.Level;

    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private HudView _hudView;
    
        [SerializeField] private UILoadScreen _uiLoadScreen;
        [SerializeField] private UIDeadScreenView _uiDeadScreen;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HudView>().FromInstance( _hudView );
            Container.BindInterfacesAndSelfTo<HudController>().AsSingle();
            Container.BindInterfacesAndSelfTo<UILoadScreen>().FromInstance( _uiLoadScreen );
            Container.BindInterfacesAndSelfTo<UIDeadScreenView>().FromInstance( _uiDeadScreen );
        }
    }
}