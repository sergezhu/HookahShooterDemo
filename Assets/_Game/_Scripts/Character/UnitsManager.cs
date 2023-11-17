namespace _Game._Scripts.Character.Enemy
{
    using System.Collections.Generic;
    using System.Linq;
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.Configs;
    using _Game._Scripts.Interaction;
    using _Game._Scripts.Level;
    using _Game._Scripts.Managers.LevelsManagement;
    using _Game._Scripts.UI.Hud;
    using _Game._Scripts.Utilities.Extensions;
    using _Game._Scripts.Weapons;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public sealed class UnitsManager : IInitializable, ITickable
    {
        private readonly LevelTargetsProvider _targetsProvider;
        private readonly CMCameraController _cmCameraController;
        private readonly LevelsController _levelsController;
        private readonly InteractionController _interactionController;
        private readonly IProjectileController _projectileController;
        private readonly HudController _hudController;
        private readonly HeroConfig _heroConfig;
        private readonly UnitsLibrary _unitsLibrary;

        private IEnemyFacade[] _enemyFacades;

        private bool _isStarted;
        private Enemy[] _enemies;
        private Dictionary<Enemy, IEnemyFacade> _enemyFacadesMap;
        private readonly CompositeDisposable _disposable;
        private readonly CompositeDisposable _enemiesDisposable;
        private readonly CompositeDisposable _areaTargetsDisposable;
        private readonly CompositeDisposable _energyConsumersDisposable;
        private Hero _hero;
        public Hero Hero => _hero;
        public int EnemiesCount => _enemies.Length;

        public ReactiveCommand<IEnemyFacade> EnemyDead { get; } = new ReactiveCommand<IEnemyFacade>();

        public UnitsManager( LevelTargetsProvider targetsProvider, CMCameraController cmCameraController, LevelsController levelsController, 
                             InteractionController interactionController, IProjectileController projectileController, HudController hudController, HeroConfig heroConfig)
        {
            _targetsProvider = targetsProvider;
            _cmCameraController = cmCameraController;
            _levelsController = levelsController;
            _interactionController = interactionController;
            _projectileController = projectileController;
            _hudController = hudController;
            _heroConfig = heroConfig;

            _disposable = new CompositeDisposable();
            _enemiesDisposable = new CompositeDisposable();
            _areaTargetsDisposable = new CompositeDisposable();
            _energyConsumersDisposable = new CompositeDisposable();
        }

        public void Initialize()
        {
            _isStarted = false;

            _levelsController.LifeCycleState
                .Where( state => state == LevelsController.LifeState.Loaded )
                .Subscribe( _ => PostInitialize() )
                .AddTo( _disposable );
            
            _levelsController.LifeCycleState
                .Where( state => state == LevelsController.LifeState.Unloading )
                .Subscribe( _ => CleanUp() )
                .AddTo( _disposable );

            Debug.Log( $"UnitsManager Initialize" );
        }

        private void CleanUp()
        {
            _hero.Deactivate();
            _hero.DetachTargetsProvider();
            
            foreach ( var enemy in _enemies )
            {
                enemy.Deactivate();
            }
            
            _disposable?.Dispose();
            _enemiesDisposable?.Dispose();
            _energyConsumersDisposable?.Dispose();
        }

        private void PostInitialize()
        {
            Debug.Log( $"UnitsManager PostInitialize" );

            _isStarted = true;

            InitializeHero();
            SubscribeHero();
            
            InitializeEnemies();
            TryActivateEnemies();
            ResubscribeEnemies();
        }

        private void InitializeEnemies()
        {
            _enemyFacades = _targetsProvider.EnemyTargets.ToArray();
            //Debug.Log( $"[InitializeEnemies] facades [{_enemyFacades.Length}]" );
            
            _enemies = _enemyFacades.Select( facade => facade.Enemy ).ToArray();
            _enemyFacadesMap = _enemies.ToDictionary( u => u, u => _enemyFacades.First( f => f.Enemy == u ) );
            //Debug.Log( $"[InitializeEnemies] enemies [{_enemies.Length}]" );
            
            _enemies.ForEach( e =>
            {
                e.DetachSpawnPoint();
                //Debug.Log( $"Enemy [{e.Name}] [{e.Category}] initialized" );
            } );
        }

        private void TryActivateEnemies()
        {
            _enemies.ForEach( e =>
            {
                var facade = _enemyFacadesMap[e];

                if ( facade.IsEnabled )
                {
                    ActivateEnemy( e, facade );
                }
            } );
        }

        private void ActivateEnemy( Enemy e, IEnemyFacade facade )
        {
            e.Activate();
            //Debug.Log( $"Enemy [{e.Name}] [{e.Category}] activated" );
        }

        private void ResubscribeEnemies()
        {
            _enemiesDisposable.Clear();
            
            foreach ( var enemy in _enemies )
            {
                enemy.RemoveSelfAsTargetRequest
                    .Subscribe( _ =>
                    {
                        var facade = _enemyFacadesMap[enemy];
                        _targetsProvider.RemoveTarget( facade );
                        
                        enemy.CleanUp();
                        
                        InitializeEnemies();
                    } )
                    .AddTo( _enemiesDisposable );

                enemy.ProjectileRequested
                    .Subscribe( data => _projectileController.HandleProjectileRequest( data ) )
                    .AddTo( _disposable );

                enemy.Dead
                    .Subscribe( _ => OnEnemyDead( enemy ) )
                    .AddTo( _enemiesDisposable );

                enemy.IsShown
                    .Subscribe( v => OnShownChanged( enemy, v ) )
                    .AddTo( _enemiesDisposable );
            }
        }

        private void OnShownChanged( Enemy enemy, bool isShown )
        {
            var facade = _enemyFacadesMap[enemy];
            
            if ( isShown )
            {
                ActivateEnemy( enemy, facade );
            }
            else
            {
                enemy.Deactivate();
            }
        }


        private void InitializeHero()
        {
            _hero = _targetsProvider.MainHeroFacade.Hero;
            _hero.AttachTargetsProvider( _targetsProvider );
            _hero.InitializeRagdoll( _targetsProvider.MainHeroFacade );

            _cmCameraController.SetHeroCamerasTarget( _hero.CameraTarget );
            _cmCameraController.SwitchToHeroCamera();
            _hero.Activate();
        }

        private void SubscribeHero()
        {
            _hero.ProjectileRequested
                .Subscribe( data =>
                {
                    Debug.Log( $"Projectile : OnProjectileRequested" );
                    _projectileController.HandleProjectileRequest( data );
                } )
                .AddTo( _disposable );

           _hero.Dead
                .Subscribe( _ => OnHeroDead() )
                .AddTo( _disposable );

            _hero.LiveState.Health
                .Subscribe( v => _hudController.SetHealth( v, _heroConfig.MaxHealth ) )
                .AddTo( _disposable );
        }

        public void Tick()
        {
        }


        private void OnHeroDead()
        {
            _hero.SetEnemiesCountHavingHeroAsTarget( 0 );
            _interactionController.HandleHeroDead();
        }

        private void OnEnemyDead( Enemy enemy )
        {
            EnemyDead.Execute( _enemyFacadesMap[enemy] );
        }

        private Enemy FacadeToModel( IEnemyFacade facade )
        {
            int index = _enemyFacadesMap.Values.ToList().FindIndex( f => f == facade );
            return index == -1 ? null : _enemyFacadesMap.Keys.ToList()[index];
        }
    }
}