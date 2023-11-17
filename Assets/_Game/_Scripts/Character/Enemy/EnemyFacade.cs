namespace _Game._Scripts.Character.Enemy
{
	using _Game._Scripts.Character.Equip;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Weapons;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IEnemyFacade : IDamageableTarget, IInteractionHintOwner
	{
		public Enemy Enemy { get; }
		public UnitEquip Equip { get; }
		public ITarget CurrentTarget { get; }

		public Vector3 Position { get; }

		bool IsAggro { get; }
		public bool IsDead { get; }
		public float FieldOfViewDistance { get; }

		bool IsAttackRollbackDoing { get; }

		ReactiveCommand RemoveSelfAsTargetRequest { get; }
		ReactiveCommand Dead { get; }

		public void AttachChild( Transform child );
		public void TryPavePath( Vector3 pos );
		public void TryFollowPath();
		void StopMove();
		public void RotateToPosition( Vector3 pos );
		public void TryStartMeleeAttack();
		public void TryStartRangeAttack();
		public void StopAttack();
		public void DestroyWithAnimate();
	}

	[SelectionBase]
	public class EnemyFacade : MonoBehaviour, IEnemyFacade
	{
		private Enemy _enemy;
		public UnitEquip Equip => _enemy.Equip;

		public string Name => _enemy.Name;
		public int SiblingIndex => transform.GetSiblingIndex();
		public bool IsEnabled => gameObject.activeSelf;
		public Enemy Enemy => _enemy;
		public ITarget CurrentTarget => _enemy.CurrentTarget;
		public Vector3 Position => _enemy.Position;
		public Quaternion Rotation => _enemy.Rotation;
		public Vector3 AimTargetPosition => _enemy.AimTargetPosition;

		public Vector3 Forward => _enemy.Forward;

		public bool IsAggro => _enemy.IsAggro;
		public bool IsDead => _enemy.IsDead;

		public float FieldOfViewDistance => _enemy.FieldOfViewDistance;
		public bool IsAttackRollbackDoing => _enemy.IsAttackRollbackDoing;

		public ReactiveCommand RemoveSelfAsTargetRequest => _enemy.RemoveSelfAsTargetRequest;
		public ReactiveCommand Dead => _enemy.Dead;
		

		[Inject] 
		private void Construct( Enemy enemy )
		{
			_enemy = enemy;
			Debug.Log( $"EnemyFacade construct : {_enemy.Name}" );
		}

		public Vector3 GetClosestPoint( Vector3 fromPos )
		{
			return Position;
		}

		public void AttachChild( Transform child )
		{
			_enemy.AttachChild( child );
		}

		public void DetachSpawnPoint()
		{
			_enemy.DetachSpawnPoint();
		}

		public void TryFollowPath()
		{
			_enemy.TryFollowPath();
		}

		public void TryPavePath( Vector3 pos )
		{
			_enemy.TryPavePath( pos );
		}

		public void StopMove()
		{
			_enemy.StopMove();
		}

		public void RotateToPosition( Vector3 pos )
		{
			_enemy.RotateToPosition( pos );
		}

		public void TryStartMeleeAttack()
		{
			Debug.Log( $"MeleeAttack!" );
			_enemy.TryMeleeAttack();
		}

		public void TryStartRangeAttack()
		{
			//Debug.Log( $"RangeAttack!" );
			_enemy.TryRangeAttack();
		}

		public void StopAttack()
		{
			//Debug.Log( $"StopAttack!" );
			_enemy.StopAttack();
		}

		public void DestroyWithAnimate()
		{
			_enemy.DestroyWithAnimate();
		}

		public void TakeDamage( Damage damage ) => _enemy.TakeDamage( damage );
		public void ShowInteractionHint() => _enemy.ShowInteractionHint();
		public void HideInteractionHint() => _enemy.HideInteractionHint();
	}
}