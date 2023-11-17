using System.Collections;
using _Game._Scripts.Managers.LevelsManagement.LevelTypes;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.Managers.LevelsManagement
{
    using _Game._Scripts.Utilities.Extensions;

    public class LevelsController : MonoBehaviour, IInitializable
    {
        public enum LifeState
        {
            Unloaded,
            Loading,
            Loaded,
            Unloading
        }
    
        [SerializeField] private Transform _levelsRoot;
        [SerializeField] private float _restartDelay;

        private ILevel _currentLevel;
        private LevelsFactory _levelsFactory;

        public ILevel CurrentLevel => _currentLevel;

        public ReactiveProperty<LifeState> LifeCycleState { get; } = new ReactiveProperty<LifeState>();


        [Inject]
        private void Construct( LevelsFactory levelsFactory)
        {
            _levelsFactory = levelsFactory;

            LifeCycleState.Value = LifeState.Unloaded;
        }

        public void Initialize()
        {
            StartCoroutine( LoadCurrentLevel() );
        }


        private IEnumerator LoadCurrentLevel()
        {
            yield return LoadLevel();
        }

        public void ReloadCurrentLevel() => StartCoroutine( ReloadCurrentLevelRoutine() );

        private IEnumerator ReloadCurrentLevelRoutine()
        {
            yield return UnloadLevel();
            yield return new WaitForSeconds( _restartDelay );
            yield return LoadLevel();
        }

        private IEnumerator LoadLevel()
        {
            LifeCycleState.Value = LifeState.Loading;

            var instantiatedLevel = _levelsFactory.CreateLevel( _levelsRoot );
            _currentLevel = instantiatedLevel;

            Subscribe();

            yield return new WaitForSeconds( 0.1f );

            SpawnHero();

            LifeCycleState.Value = LifeState.Loaded;
            Debug.Log( $"[LevelManager] Level loaded" );
        }

        private void SpawnHero()
        {
            _currentLevel.SpawnHero();
        }

        private IEnumerator UnloadLevel()
        {
            LifeCycleState.Value = LifeState.Unloading;
        
            Unsubscribe();
            _levelsRoot.DestroyChildren();

            yield return new WaitForSeconds( 0.1f );

            LifeCycleState.Value = LifeState.Unloaded;
        }

        private void Subscribe()
        {
        }

        private void Unsubscribe()
        {
        }
    }
}