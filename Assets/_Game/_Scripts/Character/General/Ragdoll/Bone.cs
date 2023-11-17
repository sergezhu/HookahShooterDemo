using UnityEngine;

namespace _Game._Scripts.Character.General.Ragdoll
{
    using _Game._Scripts.Helpers.Mono;
    using _Game._Scripts.Interfaces;

    [RequireComponent(typeof(PhysicsBody))]
    public class Bone : MonoBehaviour
    {
        public IDamageableTarget OwnerDamageableTarget { get; private set; }
        public PhysicsBody PhysicsBody { get; private set; }

        private PhysicsBody _physicsBody;

        public void Construct( IDamageableTarget owner )
        {
            OwnerDamageableTarget = owner;
            PhysicsBody = GetComponent<PhysicsBody>();
        }
    }
}
