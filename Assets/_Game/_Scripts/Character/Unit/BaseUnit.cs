namespace _Game._Scripts.Character.Unit
{
	using System;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Weapons;
	using UniRx;
	using UnityEngine;

	public class BaseUnit 
	{
		private readonly IUnitView _view;
		private readonly UnitCurrentTargetProvider _unitCurrentTargetProvider;
		private readonly UnitAttackSystem _attackSystem;
		protected readonly CompositeDisposable _disposable;

		private bool _isDamageDealingPaused;

		public BaseUnit( IUnitView view, UnitCurrentTargetProvider unitCurrentTargetProvider, UnitAttackSystem attackSystem )
		{
			_view = view;
			_unitCurrentTargetProvider = unitCurrentTargetProvider;
			_attackSystem = attackSystem;
			_disposable = new CompositeDisposable();
		}

		public string Name => _view.Name;
		
		public Vector3 Position
		{
			get => _view.Position;
			set => _view.Position = value;
		}

		public Quaternion Rotation => _view.Rotation;

		public Vector3 Forward => _view.Forward;

		public bool IsDamageDealingPaused => _isDamageDealingPaused;
		public Vector3 AimTargetPosition => _view.AimTargetPosition;
		public ReactiveCommand<ProjectileRequestData> ProjectileRequested { get; private set; }
		public ReactiveCommand Dead { get; } = new ReactiveCommand();

		public ITarget CurrentTarget
		{
			get => _unitCurrentTargetProvider.CurrentTarget;
			set => _unitCurrentTargetProvider.CurrentTarget = value;
		}

		protected ITarget CachedTarget { get; set; }

		public void Initialize()
		{
			ProjectileRequested = _attackSystem.ProjectileRequested;
		}
	}
}