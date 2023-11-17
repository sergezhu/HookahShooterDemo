namespace _Game._Scripts.Utilities.Extensions
{
	using System.Collections.Generic;
	using System.Linq;

	public static class RandomExtensions
	{
		public static List<dynamic> GetRandomList( int n )
		{
			System.Random r = new System.Random();
			List<dynamic> list = new List<dynamic>();

			for ( int i = 0; i < n; i++ )
			{
				list.Add( i );
				list[i] = r.Next();
			}

			return list;
		}
		
		public static int RandomIndex<T>( this IEnumerable<T> collection ) => UnityEngine.Random.Range( 0, collection.ToList().Count );

		public static T Random<T>( this IEnumerable<T> collection )
		{
			var list = collection.ToList();
			return list[list.RandomIndex()];
		}

		public static List<T> GetRandomList<T>( this IEnumerable<T> sourceCollection, int count )
		{
			var sourceList = sourceCollection.ToList();
			var resultList = new List<T>( count );

			for ( int i = 0; i < count; i++ ) 
				resultList.Add( sourceList.Random() );

			return resultList;
		}

		//public static int RandomIndex<T>( this List<T> list ) => UnityEngine.Random.Range( 0, list.Count );
		//public static T Random<T>( this List<T> list ) => list[list.RandomIndex()];


		public static int GetRandomWeightedIndex( this List<float> weights )
		{
			// https://forum.unity.com/threads/random-numbers-with-a-weighted-chance.442190/#post-5173340

			if ( weights == null || weights.Count == 0 )
				return -1;

			float total = 0;
			for ( int i = 0; i < weights.Count; i++ )
			{
				float w = weights[i];

				if ( float.IsPositiveInfinity( w ) )
					return i;

				if ( w > 0 && !float.IsNaN( w ) )
					total += w;
			}

			float random = UnityEngine.Random.value;

			float sum = 0;
			for ( int i = 0; i < weights.Count; i++ )
			{
				float w = weights[i];

				if ( float.IsNaN( w ) || w <= 0 )
					continue;

				sum += w / total;

				if ( sum >= random )
					return i;
			}

			return -1;
		}


		public static T PopRandom<T>( this List<T> list ) => list.Pop( list.RandomIndex() );

		public static T Pop<T>( this List<T> list, int index )
		{
			T item = list[index];
			list[index] = list[list.Count - 1];
			list.RemoveAt( list.Count - 1 );
			return item;
		}
		
	}
}