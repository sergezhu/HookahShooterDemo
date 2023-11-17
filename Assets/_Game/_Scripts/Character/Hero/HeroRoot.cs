namespace _Game._Scripts.Character.Hero
{
	using UnityEngine;

	public class HeroRoot : MonoBehaviour
	{
		public Vector3 Position => transform.position;
		public Transform Transform => transform;
	}
}