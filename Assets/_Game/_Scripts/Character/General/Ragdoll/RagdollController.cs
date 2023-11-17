using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game._Scripts.Character.General.Ragdoll
{
    using _Game._Scripts.Interfaces;
    using _Game._Scripts.Utilities.Extensions;
    using _Game._Scripts.Weapons;

    public class RagdollController : MonoBehaviour
    {
        private const int ROOT_BONE_INDEX = 0;
    
        [SerializeField] private Animator _animator;    

        [field: SerializeField] private List<Bone> _bones;

        private IDamageableTarget _ownerDamageableTarget;
        private int _deadLayer;
        private int _normalLayer;

        public Bone Root => _bones != null && _bones.Count > 0 ? _bones[ROOT_BONE_INDEX] : null;
        public bool IsActive { get; private set; }

        public void Construct( IDamageableTarget ownerDamageableTarget, int normalLayer, int deadLayer )
        {
            //Debug.Log( "[RagdollController] Construct" );
        
            _ownerDamageableTarget = ownerDamageableTarget;
            _normalLayer = normalLayer;
            _deadLayer = deadLayer;
        
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            _bones = new List<Bone>(rigidbodies.Length);

            foreach (Rigidbody rigidbody in rigidbodies)
            {
                Bone newBone = rigidbody.gameObject.AddComponent<Bone>();
                newBone.Construct( _ownerDamageableTarget ) ;
            
                _bones.Add(newBone);
            }
        }

        [Button]
        private void ClearCollidersInChildren()
        {
            var colliders = GetComponentsInChildren<Collider>(true);

            foreach ( var c in colliders ) 
                c.SmartDestroyComponent();
        }

        [Button]
        private void ClearRigidbodiesInChildren()
        {
            var rigidbodies = GetComponentsInChildren<Rigidbody>( true );

            foreach ( var rb in rigidbodies )
                rb.SmartDestroyComponent();
        }

        [Button]
        private void ClearJointsInChildren()
        {
            var joints = GetComponentsInChildren<CharacterJoint>( true );

            foreach ( var j in joints )
                j.SmartDestroyComponent();
        }

        /*public void SetLayer(int layer)
    {
        foreach ( Bone bone in _bones ) 
            bone.PhysicsBody.gameObject.layer = layer;
    }*/

        public void EnableRagdoll()
        {
            if (IsActive) return;

            _animator.enabled = false;

            foreach (Bone bone in _bones)
            {
                bone.PhysicsBody.Collider.isTrigger = false;
                bone.PhysicsBody.Rigidbody.isKinematic = false;
            }

            IsActive = true;
        }

        public void DisableRagdoll()
        {
            //Debug.Log($"Disable Ragdoll : {name} of {_ownerDamageableTarget.Name}"  );
        
            _animator.enabled = true;

            try
            {
                foreach ( Bone bone in _bones )
                {
                    bone.PhysicsBody.Collider.isTrigger = true;
                    bone.PhysicsBody.Rigidbody.isKinematic = true;
                }
            }
            catch ( Exception ex )
            {
                Debug.LogException( ex );
            }
        
            IsActive = false;
        }

        public void SetDeadAnimationSetup()
        {
            foreach ( Bone bone in _bones )
            {
                bone.PhysicsBody.Collider.isTrigger = true;
                bone.PhysicsBody.Rigidbody.isKinematic = true;
            }

            IsActive = false;
        }

        public void SetDeadLayer()
        {
            foreach ( var bone in _bones ) 
                bone.PhysicsBody.gameObject.layer = _deadLayer;
        }

        public void SetNormalLayer()
        {
            foreach ( var bone in _bones )
                bone.PhysicsBody.gameObject.layer = _normalLayer;
        }

        public void AddForceToRootBone(Vector3 force)
        {
            var rb = Root.PhysicsBody.Rigidbody;
            var t = Root.PhysicsBody.transform;
            rb.AddForce(force, ForceMode.Impulse);

            Debug.DrawLine( t.position + 2 * Vector3.up, t.position + 8f * force.normalized + 2 * Vector3.up, Color.red, 1f );
        
            Debug.Log( $"AddForceToRootBone : {Root.PhysicsBody.name}, force {force} (mass {rb.mass})" );
        }

        public void Push( Damage damage )
        {
            AddForceToRootBone( damage.HitForce );
        }

        public void Push( Vector3 hitForce )
        {
            AddForceToRootBone( hitForce );
        }

        private void OnValidate()
        {
            _animator = GetComponentInChildren<Animator>();
            _ownerDamageableTarget = GetComponentInParent<IDamageableTarget>();
        }
    }
}