using _Game._Scripts.Managers.Input;
using Cinemachine;
using QFSW.MOP2;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Installers
{
    using _Game._Scripts.Level;
    using _Game._Scripts.Managers;
    using _Game._Scripts.Managers.LevelsManagement;
    using _Game._Scripts.UI;
    using _Game._Scripts.Utilities;

    public class ServicesInstaller : MonoInstaller
    {
        [SerializeField] private LevelsController _levelsController;
        [SerializeField] private MasterObjectPooler _pooler;
        [SerializeField] private UIController _uiController;

        [Header("Camera")]
        [SerializeField] private CMCameraController _cmCameraController;

        [SerializeField] private CinemachineBrain _cmBrain;

        [Header( "Input" )]
        [SerializeField] private Joystick _leftZoneJoystick;
        [SerializeField] private Joystick _rightZoneJoystick;

        public override void InstallBindings()
        {
            Debug.Log( $"[Install] ServicesInstaller : InstallBindings" );
        
            Container
                .BindInterfacesTo<TapticController>()
                .AsSingle();

            Container
                .Bind<SystemPauseService>()
                .AsSingle();
        
            Container
                .Bind<MasterObjectPooler>()
                .FromInstance( _pooler );

            InstallCamera();
            InstallInput();

            Container
                .Bind<LevelsFactory>()
                .AsSingle();
        
            Container
                .BindInterfacesAndSelfTo<LevelsController>()
                .FromInstance(_levelsController);

            Container
                .BindInterfacesAndSelfTo<NavMeshProvider>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<UIController>()
                .FromInstance( _uiController );
        }

        private void InstallInput()
        {
            Debug.Log( $"[Install] ServicesInstaller : InstallInput" );

            Container
                .BindInterfacesTo<InputManager>()
                .AsSingle();

            Container
                .Bind<Joystick>()
                .WithId( "Left" )
                .FromInstance( _leftZoneJoystick );

            Container
                .Bind<Joystick>()
                .WithId( "Right" )
                .FromInstance( _rightZoneJoystick );
        
            Container
                .BindInterfacesAndSelfTo<JoystickInput>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<KeyboardInput>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<DevInput>()
                .AsSingle();
        }

        private void InstallCamera()
        {
            Debug.Log( $"[Install] ServicesInstaller : InstallCamera" );

        
            Container
                .BindInterfacesAndSelfTo<ScreenResizeDetector>()
                .AsSingle();
        
            Container
                .Bind<CinemachineBrain>()
                .FromInstance( _cmBrain );
        
            Container
                .BindInterfacesAndSelfTo<CMCameraController>()
                .FromInstance( _cmCameraController );

            Container
                .BindInterfacesAndSelfTo<CameraRotator>()
                .AsSingle();
        }

        private void InstallDebugTools()
        {
        }
    }
}