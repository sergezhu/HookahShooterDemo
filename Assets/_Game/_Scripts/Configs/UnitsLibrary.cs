namespace _Game._Scripts.Configs
{
	using _Game._Scripts.Character.Hero;
	using UnityEngine;

	[CreateAssetMenu( fileName = "UnitsLibrary", menuName = "Configs/UnitsLibrary" )]
	public class UnitsLibrary : ScriptableObject
	{
		[SerializeField] private HeroFacade _heroPrefab;

		public HeroFacade HeroPrefab => _heroPrefab;
	}
}