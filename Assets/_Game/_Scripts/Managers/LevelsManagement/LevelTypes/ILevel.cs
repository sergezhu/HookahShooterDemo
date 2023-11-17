namespace _Game._Scripts.Managers.LevelsManagement.LevelTypes
{
	using UnityEngine.AI;

	public interface ILevel
	{
		string Name { get; }
		NavMeshSurface NavMeshSurface { get; }
		
		void SpawnHero();
	}
}