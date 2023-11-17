namespace _Game._Scripts.Installers
{
	using _Game._Scripts.Character;
	using _Game._Scripts.Character.Enemy;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Interaction;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Level;
	using _Game._Scripts.MapGenerator;
	using _Game._Scripts.Weapons;
	using UnityEngine.AI;
	using Zenject;

	public class LevelInstaller: MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.Bind<NavMeshSurface>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.Bind<UnitsFactory>()
				.AsSingle();

			Container
				.Bind<HeroRoot>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.Bind<ITarget>()
				.FromComponentsInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<LevelTargetsProvider>()
				.FromComponentInHierarchy()
				.AsSingle();



			Container
				.BindInterfacesAndSelfTo<ProjectileController>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<UnitsManager>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<InteractionController>()
				.AsSingle();
		}
	}
}