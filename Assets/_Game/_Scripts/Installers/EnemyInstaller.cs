namespace _Game._Scripts.Installers
{
	using _Game._Scripts.Character.Animations;
	using _Game._Scripts.Character.Enemy;
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Character.General.Movement;
	using _Game._Scripts.Character.General.Ragdoll;
	using _Game._Scripts.Character.Unit;
	using _Game._Scripts.InventoryItemsServices.WorldItemViews;
	using _Game._Scripts.Level;
	using _Game._Scripts.StateMachine;
	using _Game._Scripts.StateMachine.Base;
	using _Game._Scripts.Utilities;
	using _Game._Scripts.Weapons;
	using Zenject;

	public class EnemyInstaller: MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.Bind<EnemyConfig>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<CoroutineRunner>()
				.FromComponentInHierarchy()
				.AsSingle();
			
			Container
				.BindInterfacesAndSelfTo<WeaponView>()
				.FromComponentsInHierarchy( includeInactive: true )
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<UnitCurrentTargetProvider>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<UnitEquip>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<EnemyLiveState>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<EnemyView>()
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
				.BindInterfacesAndSelfTo<EnemyAIStateMachine>()
				.AsSingle();

			Container
				.Bind<AIStateFactory>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<CharacterMovement>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<EnemyAIController>()
				.AsSingle();

			Container
				.BindInterfacesAndSelfTo<Enemy>()
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
				.BindInterfacesAndSelfTo<EnemyFacade>()
				.FromComponentInHierarchy()
				.AsSingle();
		}
	}
}