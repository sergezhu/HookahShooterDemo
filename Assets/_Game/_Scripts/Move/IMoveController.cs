namespace _Game._Scripts.Move
{
	using UnityEngine;

	public interface IMoveController
	{
		void Tick();
		void Activate();
		void Deactivate();
		void Lock();
		void Unlock();
		void CleanUp();

		bool IsActive { get; }
		Vector3 Position { get; }
		Vector3 Direction { get; }
		bool TryFollowPath();
	}
}