using UnityEngine;

namespace _Game._Scripts.Interfaces
{
    public interface ITarget
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Forward { get; }
        string Name { get; }
        bool IsEnabled { get; }
        int SiblingIndex { get; }
    
        Vector3 GetClosestPoint( Vector3 fromPos );
    }
}
