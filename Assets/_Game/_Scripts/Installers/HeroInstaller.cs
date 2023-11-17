namespace _Game._Scripts.Installers
{
	using _Game._Scripts.Character.Animations;
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Character.General.Movement;
	using _Game._Scripts.Character.General.Ragdoll;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Character.Unit;
	using _Game._Scripts.InventoryItemsServices.WorldItemViews;
	using _Game._Scripts.Level;
	using _Game._Scripts.Managers.Input;
	using _Game._Scripts.StateMachine;
	using _Game._Scripts.StateMachine.Base;
	using _Game._Scripts.Utilities;
	using _Game._Scripts.Weapons;
	using Zenject;

	public class HeroInstaller: MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesAndSelfTo<HeroLiveState>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<UnitCurrentTargetProvider>()
				.AsSingle();
			
			Container
				.BindInterfacesAndSelfTo<UnitEquip>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<HeroAnimationEventsProvider>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<WeaponView>()
				.FromComponentsInHierarchy( includeInactive: true )
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<HeroInput>()
				.AsSingle();
			
			Container
				.BindInterfacesAndSelfTo<CoroutineRunner>()
				.FromComponentInHierarchy()
				.AsSingle();
			
			Container
				.Bind<RagdollController>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<HeroView>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<UnitAnimatorController>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<UnitAttackSystem>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<ItemViewsController>()
				.FromComponentInHierarchy()
				.AsSingle();


			Container
				.BindInterfacesAndSelfTo<HeroAIStateMachine>()
				.AsSingle();

			Container
				.Bind<AIStateFactory>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<CharacterMovement>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<HeroAIController>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<Hero>()
				.AsSingle();

			InstallFacade();

			Container
				.Bind<TargetColliderTag>()
				.FromComponentInHierarchy()
				.AsSingle();
		}

		public void InstallFacade()
		{
			Container
				.BindInterfacesAndSelfTo<HeroFacade>()
				.FromComponentInHierarchy()
				.AsSingle();
		}
	}
}