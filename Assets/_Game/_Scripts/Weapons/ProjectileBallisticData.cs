using UnityEngine;

namespace _Game._Scripts.Weapons
{
    [CreateAssetMenu( fileName = "ProjectileBallisticData", menuName = "Configs/Projectile/ProjectileBallisticData" )]
    public class ProjectileBallisticData : ScriptableObject
    {
        //[SerializeField] private ObjectPool _pool;
        //[SerializeField] private ObjectPool _hitFXPool;
        //[SerializeField] private ObjectPool _decalFXPool;

        [Space]
        [SerializeField] private float _speed;
        [SerializeField] private float _gravityFactor;
        [SerializeField] private float _startAngleAboveHorizon;

        //public string PoolName => _pool.PoolName;
        //public string HitFXPoolName => _hitFXPool == null ? "" : _hitFXPool.PoolName;
        //public string DecalFXPoolName => _decalFXPool == null ? "" : _decalFXPool.PoolName;

        public float Speed => _speed;
        public float GravityFactor => _gravityFactor;
        public float StartAngleAboveHorizon => _startAngleAboveHorizon;
    }
}
