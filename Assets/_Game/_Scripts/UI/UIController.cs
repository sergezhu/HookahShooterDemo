using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Game._Scripts.UI
{
    using _Game._Scripts.Managers.LevelsManagement;
    using _Game._Scripts.UI.Level;
    using _Game._Scripts.Utilities.Extensions;

    public class UIController : MonoBehaviour
    {
        [Inject] private LevelsController _levelsController;
        [Inject] private UILoadScreen _uiLoadScreen;
        [Inject] private UIDeadScreenView _uiDeadScreen;


        private CompositeDisposable _uiDeadScreenDisposable;
    

        private void Start()
        {
            _uiDeadScreen.Hide(); 
        
            _uiDeadScreenDisposable = new CompositeDisposable();
        
            _levelsController.LifeCycleState
                .Where(state => _levelsController.LifeCycleState.Value == LevelsController.LifeState.Unloading)
                .Subscribe(_ => OnChunkUnloading())
                .AddTo(this);

            _levelsController.LifeCycleState
                .Where( state => _levelsController.LifeCycleState.Value == LevelsController.LifeState.Loaded )
                .Subscribe( _ => OnChunkLoaded() )
                .AddTo( this );

            _uiDeadScreen.RestartClick
                .Where( state => _levelsController.LifeCycleState.Value == LevelsController.LifeState.Loaded )
                .Subscribe( _ => OnRestartClick() )
                .AddTo( this );
        }

        private void OnRestartClick()
        {
            _uiDeadScreen.Hide();
            _levelsController.ReloadCurrentLevel();
        }

        private void OnChunkLoaded()
        {
            _uiLoadScreen.FadeOut();
        }

        private void OnChunkUnloading()
        {
            _uiDeadScreenDisposable.Clear();
            _uiLoadScreen.FadeIn();
        }

        public void ShowDeadScreen( float delay )
        {
            Observable
                .NextFrame()
                .Delay( TimeSpan.FromSeconds( delay ) )
                .Subscribe( _ => _uiDeadScreen.Show() )
                .AddTo( _uiDeadScreenDisposable );
        }
    }
}
