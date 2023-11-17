#if UNITY_EDITOR

namespace _Game._Scripts.Utilities.EditorHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using _Game._Scripts.Utilities.Extensions;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    public class EditorShowMissingInChildren : MonoBehaviour
    {
        [SerializeField] private List<Transform> _result;

        [Button]
        private void Find()
        {
            var transforms = GetComponentsInChildren<Transform>(true);

            _result = transforms.Where(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0)
                .ToList();
        }

        [Button]
        private void Clean()
        {
            _result
                .Select(t => t.gameObject)
                .ForEach(go => GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go));
        }
    }
}

#endif
