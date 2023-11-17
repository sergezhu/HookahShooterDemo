namespace _Game._Scripts.Character
{
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Configs;
	using UnityEngine;
	using Zenject;

	public class UnitsFactory
	{
		private readonly UnitsLibrary _unitsLibrary;
		private readonly IInstantiator _instantiator;

		public UnitsFactory(UnitsLibrary unitsLibrary, IInstantiator instantiator)
		{
			_unitsLibrary = unitsLibrary;
			_instantiator = instantiator;
		}

		public IHeroFacade CreateHero(Vector3 position, Quaternion rotation, Transform parent)
		{
			var instantiatedHero = _instantiator
				.InstantiatePrefab( _unitsLibrary.HeroPrefab, position, rotation, parent )
				.GetComponent<IHeroFacade>();

			return instantiatedHero;
		}
	}
}