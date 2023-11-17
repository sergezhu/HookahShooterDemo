namespace _Game._Scripts.Character
{
	using DG.Tweening;
	using UniRx;

	public class RollbackWeaponTimer
	{
		public bool IsAttackRollbackDoing { get; private set; }

		private Tween _attackRollbackTween;

		public ReactiveCommand<float> RollbackProgressChanged { get; } = new ReactiveCommand<float>();
		
		public void SetRollback( float rollBackAttackDuration )
		{
			_attackRollbackTween?.Kill();
			IsAttackRollbackDoing = true;

			_attackRollbackTween = DOVirtual.Float( 0, 1f, rollBackAttackDuration, value => { RollbackProgressChanged.Execute( value ); } )
				.OnComplete( () =>
				{
					IsAttackRollbackDoing = false;
					_attackRollbackTween = null;

					RollbackProgressChanged.Execute( 1 );
				} );
		}
	}
}