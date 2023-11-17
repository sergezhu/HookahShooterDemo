namespace _Game._Scripts.Character.Unit
{
	using UniRx;

	public interface IUnitLiveState
	{
		ReadOnlyReactiveProperty<float> Health { get; }
		public float LastHealthDelta { get; }
		bool IsHealthFull { get; }

		void ChangeHealth( float value );
		void Ressurect();
	}
}