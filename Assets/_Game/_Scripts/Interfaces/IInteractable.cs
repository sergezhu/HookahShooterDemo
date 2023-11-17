namespace _Game._Scripts.Interfaces
{
	using UniRx;

	public interface IInteractable
	{
		bool CanInteraction { get; }
		float InteractionDuration { get; }
		//IAreaTarget InteractionArea { get; }
		ReactiveCommand<IInteractable> InteractionEnded { get; }
		
		void StartInteraction();
		void EndInteraction();
		void CancelInteraction();
		void Activate();
		void Deactivate();
	}
}