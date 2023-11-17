namespace _Game._Scripts.Level
{
	using _Game._Scripts.Character.Enemy;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Interfaces;

	public static class TargetConditions
	{
		public static ITarget Hero { get; set; }
		
		public static bool IsDeadEnemy( ITarget target )
		{
			return target is IHeroFacade { IsDead: true };
		}

		public static bool IsAggressiveEnemy( ITarget target )
		{
			return target is IEnemyFacade enemy && enemy.IsDead == false && enemy.CurrentTarget == Hero;
		}

		public static bool IsLiveEnemy( ITarget target )
		{
			return target is IEnemyFacade { IsDead: false };
		}

		public static bool IsAnyEnemy( ITarget target )
		{
			return target is IEnemyFacade;
		}

		public static bool IsDeadHero( ITarget target )
		{
			return target is IHeroFacade { IsDead: true };
		}

		public static bool IsLiveHero( ITarget target )
		{
			return target is IHeroFacade { IsDead: false };
		}

		public static bool IsAnyHero( ITarget target )
		{
			return target is IHeroFacade;
		}

		public static bool IsAny( ITarget target )
		{
			return true;
		}
	}
}