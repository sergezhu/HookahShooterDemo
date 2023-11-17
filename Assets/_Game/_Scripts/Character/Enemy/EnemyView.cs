namespace _Game._Scripts.Character.Enemy
{
    using System;
    using System.Linq;
    using _Game._Scripts.Character.Unit;
    using _Game._Scripts.Navigation;
    using _Game._Scripts.UI.Hud;
    using _Game._Scripts.Utilities.Extensions;
    using _Game._Scripts.Weapons;
    using DG.Tweening;
    using UnityEngine;
    using Zenject;

    public interface IEnemyView : IUnitView
    {
    }
    
    public sealed class EnemyView : MonoBehaviour, IEnemyView, IInitializable
    {
        [SerializeField] private Transform _aimTarget;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private CustomNavMeshAgent _navMeshAgent;
        [SerializeField] private GameObject _selectedTargetHint;
        [SerializeField] private UnitHealthView _healthView;

        private Transform _transform;
        private WeaponView[] _weaponViews;
        private EnemyConfig _enemyConfig;
        private string _name;
        private Tween _destroyAnimation;

        public string Name => _name;

        public Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }


        public Quaternion Rotation => Transform.rotation;
        public Vector3 Forward => Transform.forward;
        public Vector3 Right => Transform.right;
        public CustomNavMeshAgent NavMeshAgent => _navMeshAgent;
        public float Radius => _capsuleCollider.radius;
        public bool IsShown => gameObject.activeSelf;

        private Transform Transform => _transform ??= transform;
        public Vector3 AimTargetPosition => _aimTarget.position;

        public int SiblingIndex => transform.GetSiblingIndex();



        [Inject]
        private void Construct( WeaponView[] weaponViews, EnemyConfig enemyConfig )
        {
            _weaponViews = weaponViews;
            _enemyConfig = enemyConfig;

            _name = name;
            _healthView.Construct( _enemyConfig.MaxHealth );

            Debug.Log( $"WeaponViews : {_weaponViews.Length}" );
        }


        public void Initialize()
        {
            HideInteractionHint();
        }

        public void AttachChild( Transform child )
        {
            child.SetParent( Transform );
        }

        public void ShowInteractionHint()
        {
            _selectedTargetHint.Show();
        }

        public void HideInteractionHint()
        {
            _selectedTargetHint.Hide();
        }

        public WeaponView GetWeaponViewByName( string weaponName )
        {
            return _weaponViews.FirstOrDefault( view => string.Equals( view.ItemName, weaponName ) );
        }

        public void SetHealth( float healthValue )
        {
            _healthView.SetHealth( healthValue );
        }

        public void DestroyWithAnimate(Action onComleteBeforeDestroy)
        {
            _destroyAnimation?.Kill();
            
            _destroyAnimation = Transform
                .DOMoveY( Transform.localPosition.y - _enemyConfig.DestroyOffsetY, _enemyConfig.DestroyAfterDeadDuration )
                .SetDelay( _enemyConfig.DestroyAfterDeadDuration )
                .SetEase( Ease.InOutCubic )
                .OnStart( () =>
                {
                    onComleteBeforeDestroy?.Invoke();
                } )
                .OnComplete( () =>
                {
                    gameObject.SetActive( false );
                    Destroy( gameObject, 0.1f );
                } );
        }

        private void OnDestroy()
        {
            _destroyAnimation?.Kill();
        }
    }
}