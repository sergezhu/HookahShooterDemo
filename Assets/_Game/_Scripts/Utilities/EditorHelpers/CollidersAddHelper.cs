namespace _Game._Scripts.Utilities.EditorHelpers
{
	using System.Collections.Generic;
	using _Game._Scripts.Utilities.Extensions;
	using Sirenix.OdinInspector;
	using UnityEngine;

	public class CollidersAddHelper : MonoBehaviour
	{
		[SerializeField] private List<Collider> _colliders;

		[Button]
		private void RemoveCollidersInChildrenMeshes()
		{
			var meshes = GetComponentsInChildren<MeshFilter>(true);

			foreach ( var mesh in meshes )
			{
				while ( mesh.TryGetComponent( out Collider collider ) )
				{
					if ( collider != null )
						collider.SmartDestroyComponent();
				}
			}
		}

		[Button]
		private void AddMeshCollidersInChildrenMeshes()
		{
			var meshes = GetComponentsInChildren<MeshFilter>( true );

			foreach ( var mesh in meshes )
			{
				if ( mesh.TryGetComponent( out Collider collider ) )
					continue;

				mesh.gameObject.AddComponent<MeshCollider>();
			}
		}
	}
}