namespace _Game._Scripts.Character.Unit
{
	using UniRx;

	public interface IAttackEventsProvider
	{
		ReactiveCommand MeleeHit { get; }
		ReactiveCommand RangeFire { get; }
		void OnHit();
	}
}