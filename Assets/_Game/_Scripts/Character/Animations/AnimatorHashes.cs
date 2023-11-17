namespace _Game._Scripts.Character.Animations
{
	using UnityEngine;

	public class AnimatorHashes
	{
		public static readonly int CommonVelocity = Animator.StringToHash( "CommonVelocity" );
		public static readonly int ForwardVelocity = Animator.StringToHash( "ForwardVelocity" );
		public static readonly int RightVelocity = Animator.StringToHash( "RightVelocity" );
		public static readonly int AttackSpeedMultiplier = Animator.StringToHash( "AttackSpeedMultiplier" );
		public static readonly int MoveAnimationMultiplier = Animator.StringToHash( "MoveAnimationMultiplier" );
		public static readonly int NormalMoveAnimationMultiplier = Animator.StringToHash( "NormalMoveAnimationMultiplier" );
		public static readonly int Idle = Animator.StringToHash( "Idle" );
		public static readonly int Aim = Animator.StringToHash( "Aim" );
		public static readonly int IsAttacking = Animator.StringToHash( "IsAttacking" );
		public static readonly int MeleeAttackTrigger = Animator.StringToHash( "MeleeAttackTrigger" );
		public static readonly int IsReloading = Animator.StringToHash( "IsReloading" );
		public static readonly int IsAiming = Animator.StringToHash( "IsAiming" );
		public static readonly int ShootTrigger = Animator.StringToHash( "ShootTrigger" );
		public static readonly int WeaponID = Animator.StringToHash( "WeaponID" );
		public static readonly int DeadTrigger = Animator.StringToHash( "DeadTrigger" );
		public static readonly int DeadID = Animator.StringToHash( "DeadID" );
	}
}