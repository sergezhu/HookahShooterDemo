namespace _Game._Scripts.Utilities.EditorHelpers
{
	using System.Collections.Generic;
	using System.Linq;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities;
	using UnityEngine;

	public class RenderersHelper : MonoBehaviour
	{
		[SerializeField, ReadOnly] private Renderer[] _renderers;
		
		private Renderer[] _renderersWithDinamicOcclusion;


		[Button]
		private void FindInChildren()
		{
			_renderers = GetComponentsInChildren<Renderer>( true );
		}

		[Button]
		private void LogDynamicOcclusion()
		{
			_renderersWithDinamicOcclusion = _renderers.Where( r => r.allowOcclusionWhenDynamic == true ).ToArray();
			
			Debug.Log( $"With dynamic occlusion : ( {_renderersWithDinamicOcclusion.Length} / {_renderers.Length} )" ); 
		}

		[Button]
		private void LogUniqueMaterials()
		{
			var uniqueMaterials = GetUniqueMaterials()
				.Select( mat => $"{mat.name}\n" )
				.ToArray();

			var concat = string.Concat( uniqueMaterials );

			Debug.Log( $"UniqueMaterials : \n{concat}" );
		}

		public IEnumerable<Material> GetUniqueMaterials()
		{
			return _renderers
				.Select( r => r.sharedMaterial )
				.Distinct()
				.Where( mat => mat != null );
		}

		[Button]
		private void DisableDynamicOcclusion()
		{
			_renderers.ForEach( r => r.allowOcclusionWhenDynamic = false );
		}

		[Button]
		private void EnableDynamicOcclusion()
		{
			_renderers.ForEach( r => r.allowOcclusionWhenDynamic = true );
		}
	}
}