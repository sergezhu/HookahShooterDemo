namespace _Game._Scripts.Interfaces
{
	using UniRx;
	using UnityEngine;

	public interface IAreaTarget : IMovingTarget
	{
		ReactiveCommand<ITarget> Enter { get; }
		ReactiveCommand<ITarget> Exit { get; }
		ReadOnlyReactiveProperty<bool> IsInside { get; }
		GameObject DisposedAreaTarget { get; }
	}
}