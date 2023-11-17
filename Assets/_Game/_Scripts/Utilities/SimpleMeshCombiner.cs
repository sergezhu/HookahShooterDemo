namespace _Game._Scripts.Utilities
{
	using System.Collections.Generic;
	using System.Linq;
	using _Game._Scripts.Utilities.EditorHelpers;
	using _Game._Scripts.Utilities.Extensions;
	using UnityEngine;

	public class SimpleMeshCombiner : MonoBehaviour
	{
		[SerializeField] private Transform _sourceRoot;
		[SerializeField] private Transform _outputRoot;

		private List<Material> _outputMaterials;
		private List<string> _outputMaterialsNames;
		private Dictionary<string, Transform> _outputRootsMap;

		private void Start()
		{
			_outputMaterialsNames = new List<string>();
			_outputMaterials = new List<Material>();
			_outputRootsMap = new Dictionary<string, Transform>();

			MeshFilter[] meshFilters = _sourceRoot.GetComponentsInChildren<MeshFilter>();

			for ( int i = 0; i < meshFilters.Length; i++ )
			{
				var meshFilter = meshFilters[i];
				var renderer = meshFilter.GetComponent<Renderer>();
				
				if(renderer == null)
					continue;

				if ( renderer.sharedMaterial == null )
					continue;

				var sharedMaterial = renderer.sharedMaterial;
				
				if ( _outputMaterialsNames.Contains( sharedMaterial.name ) == false )
				{
					var outputMaterial = new Material( sharedMaterial );
					_outputMaterials.Add( outputMaterial );
					_outputMaterialsNames.Add( outputMaterial.name );

					Transform newOutputGroupRoot = (new GameObject( sharedMaterial.name )).transform;
					newOutputGroupRoot.SetParent( _outputRoot );

					_outputRootsMap[sharedMaterial.name] = newOutputGroupRoot;
				}

				meshFilter.transform.SetParent( _outputRootsMap[sharedMaterial.name] );
			}

			_outputRootsMap.Values.ForEach( ( t, index ) =>
			{
				var outputRootMeshFilter = t.gameObject.AddComponent<MeshFilter>();
				var outputRootMeshRenderer = t.gameObject.AddComponent<MeshRenderer>();
				var collider = t.gameObject.AddComponent<MeshCollider>();
				collider.convex = false;

				var mesh = CombineChildren( t );

				outputRootMeshFilter.mesh = mesh;
				collider.sharedMesh = mesh;
				outputRootMeshRenderer.sharedMaterial = _outputMaterials[index];
			} );
		}

		private Mesh CombineChildren(Transform groupRoot)
		{
			MeshFilter[] meshFilters = groupRoot.GetComponentsInChildren<MeshFilter>();

			var combine = new CombineInstance[meshFilters.Length];

			for ( int i = 0; i < meshFilters.Length; i++ )
			{
				var meshFilter = meshFilters[i];
				combine[i].mesh = meshFilter.sharedMesh;
				combine[i].transform = meshFilter.transform.localToWorldMatrix;
				meshFilters[i].gameObject.SetActive( false );
			}

			var mesh = new Mesh();
			mesh.CombineMeshes( combine );

			groupRoot.GetComponent<MeshFilter>().sharedMesh = mesh;
			groupRoot.gameObject.SetActive( true );
			
			return mesh;
		}
	}
}