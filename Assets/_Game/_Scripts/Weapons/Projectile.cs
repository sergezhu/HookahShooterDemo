namespace _Game._Scripts.Weapons
{
	using System;
	using System.Collections.Generic;
	using _Game._Scripts.Character.General.Ragdoll;
	using _Game._Scripts.FX;
	using _Game._Scripts.Interfaces;
	using _Game._Scripts.Utilities.Extensions;
	using UniRx;
	using UnityEngine;
	using Random = UnityEngine.Random;

	public struct ProjectileHitData
	{
		public float Damage;
		public IDamageable DamageableTarget;
		public Vector3 HitPosition;
		public Vector3 HitDirection;
		public float HitForceModifier;
		public bool HitAreaDamageEnabled;
		public float HitAreaDamageRadius;
		public float HitAreaDamageValue;
		public string HitFxPoolName;
		public string AreaHitFxPoolName;
		public string HitFxDecalName;
	}

	public enum ProjectileOwner
	{
		None,
		
		Player,
		Enemy
	}
	
	[RequireComponent( typeof(Collider), typeof(Rigidbody) )]
	public class Projectile : MonoBehaviour
	{
		[SerializeField] private Transform _model;
		[SerializeField] private Collider _collider;
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private float _checkRadius;
		[SerializeField] private FXWrapper _fx;
		[SerializeField] private ProjectileBallisticData _ballisticData;

		private Transform _transform;

		private Vector3 _currentPos;
		private Vector3 _previousPos;
		private int _index;
		private int _mask;
		private bool _isHit;
		private bool _isLaunched;
		private bool _isPaused;
		private RaycastHit[] _hits;
		private List<Vector3> _debugTrajectory;
		private ProjectileRequestData _requestData;
		private ProjectilePools _pools;


		public ReactiveCommand<ProjectileHitData> Hit { get; } = new ReactiveCommand<ProjectileHitData>();
		public ReactiveCommand OutOfRange { get; } = new ReactiveCommand();

		public uint WeaponID => _requestData.DatabaseWeapon.ID;
		public int Mask => _mask;
		public FXWrapper FX => _fx;


		public void Construct( ProjectileRequestData requestData, int mask, ProjectilePools pools, int index )
		{
			_requestData = requestData;
			
			_transform = transform;
			_pools = pools;
			_index = index;
			
			_rigidbody.isKinematic = false;
			_rigidbody.useGravity = false;
			_collider.isTrigger = true;
			_isLaunched = false;
			_isHit = false;

			_hits = new RaycastHit[10];
			_mask = mask;
			_debugTrajectory = new List<Vector3>();
		}

		public void AlignToParent(Transform parent)
		{
			var scatter = _requestData.DatabaseWeapon.RandomAngleScatter;
			var rndScatter = scatter * Random.onUnitSphere.WithZ( 0 );

			var t = transform;
			var prevParent = t.parent;

			t.parent = parent;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.Euler( 0, rndScatter.y, 0 );
			t.parent = prevParent;
		}

		public void AlignToParent( Transform parent, Vector3 targetPosition )
		{
			var randomScatter = _requestData.DatabaseWeapon.RandomAngleScatter;
			var rndScatterMultiplier = randomScatter * Random.onUnitSphere;

			var staticScatterBaseAngle = _requestData.DatabaseWeapon.StaticAngleScatter;
			var separateDamageCount = _requestData.DatabaseWeapon.SeparateDamageCount;
			var staticScatterAngleY= 0f;
			
			
			if ( separateDamageCount > 1 )
			{
				staticScatterAngleY = Mathf.Lerp( -0.5f * staticScatterBaseAngle, 0.5f * staticScatterBaseAngle, (float)_index / (separateDamageCount - 1));
			}

			var t = transform;
			var prevParent = t.parent;

			var dir = (targetPosition - parent.position).normalized;

			t.parent = parent;
			t.localPosition = Vector3.zero;
			t.forward = dir;
			t.localRotation = Quaternion.Euler( 0, rndScatterMultiplier.y + staticScatterAngleY, 0 ) * t.localRotation;
			t.parent = prevParent; 
		}

		public void StartFly()
		{
			if(_isLaunched)
				return;
			
			_model.Show();

			_isLaunched = true;
			_currentPos = _rigidbody.position;
			_previousPos = _currentPos;

			var startAboveHorizonRotate = Quaternion.Euler( -1 * _ballisticData.StartAngleAboveHorizon, 0, 0 );

			_rigidbody.velocity = _ballisticData.Speed * (startAboveHorizonRotate *_transform.forward);
			_debugTrajectory.Clear();
			
			_fx.PlayProperly();
		}

		public void EndFly()
		{
			_model.Hide();
			
			_fx.Stop();
			_isLaunched = false;
			_rigidbody.velocity = Vector3.zero;
		}

		private void FixedUpdate()
		{
			if ( _isHit || _isPaused || _isLaunched == false )
				return;

			UpdateGravityVelocity();
			//CheckIfRaycastHit();
			CheckIfSphereHit();
			
			_debugTrajectory.Add( _currentPos );

			if ( InRange( _currentPos ) == false )
				OutOfRange.Execute();
		}

		private void UpdateGravityVelocity()
		{
			_rigidbody.velocity += -10 * _ballisticData.GravityFactor * Time.fixedDeltaTime * Vector3.up;
		}

		private void CheckIfRaycastHit()
		{
			_previousPos = _currentPos;
			_currentPos = _rigidbody.position;

			var dir = (_currentPos - _previousPos).normalized;
			var dist = (_currentPos - _previousPos).magnitude;

			var count = Physics.RaycastNonAlloc( _previousPos, dir, _hits, dist, _mask );

			foreach ( var hit in _hits )
			{
				if(hit.collider == null)
					break;

				if ( _isHit == false && _isLaunched )
					HandleCollision( hit, dir );
			}
		}

		private void CheckIfSphereHit()
		{
			_previousPos = _currentPos;
			_currentPos = _rigidbody.position;

			var vector = _currentPos - _previousPos;
			var dir = vector.normalized;
			var dist = vector.magnitude;

			var count = Physics.SphereCastNonAlloc( _previousPos, _checkRadius, dir, _hits, dist, _mask );

			foreach ( var hit in _hits )
			{
				if ( hit.collider == null )
					break;

				if ( _isHit == false && _isLaunched )
					HandleCollision( hit, dir );
			}
		}

		private void HandleCollision( RaycastHit hit, Vector3 dir )
		{
			hit.transform.TryGetComponent<IDamageable>( out var damageable );

			if ( damageable == null )
			{
				hit.transform.TryGetComponent<Bone>( out var bone );

				if ( bone != null )
					damageable = bone.OwnerDamageableTarget;
			}

			string damageableName = damageable == null ? "" : ((ITarget)damageable).Name;

			var hitFxPool = _pools.GetHitFxPool( WeaponID );
			var areaHitFxPool = _pools.GetAreaHitFxPool( WeaponID );
			var hitDecalPool = _pools.GetHitDecalPool( WeaponID );

			try
			{
				var data = new ProjectileHitData()
				{
					Damage = _requestData.Damage / _requestData.DatabaseWeapon.SeparateDamageCount,
					DamageableTarget = damageable,
					HitPosition = hit.point,
					HitDirection = dir,
					HitForceModifier = _requestData.HitForceModifier / _requestData.DatabaseWeapon.SeparateDamageCount,
					HitAreaDamageEnabled = _requestData.DatabaseWeapon.HitAreaDamageEnabled,
					HitAreaDamageRadius = _requestData.DatabaseWeapon.HitAreaDamageRadius,
					HitAreaDamageValue = _requestData.DatabaseWeapon.HitAreaDamageValue,

					HitFxPoolName = hitFxPool == null ? string.Empty : hitFxPool.name,
					AreaHitFxPoolName = areaHitFxPool == null ? string.Empty : areaHitFxPool.name,
					HitFxDecalName = hitDecalPool == null ? string.Empty : hitDecalPool.name,
				};

				Debug.Log( $"<color=red>{name} collided with {hit.collider.name}, dir : {dir}, " +
						   $"area hit enable : {_requestData.DatabaseWeapon.HitAreaDamageEnabled}, " +
						   $"area hit value : {_requestData.DatabaseWeapon.HitAreaDamageValue}, " +
						   $"area hit radius : {_requestData.DatabaseWeapon.HitAreaDamageRadius}, " +
						   $"damageableName : [{damageableName}], " +
						   $"hitFx : [{data.HitFxPoolName}], " +
						   $"areaHitFx : [{data.AreaHitFxPoolName}], " +
						   $"</color>" );

				Hit.Execute( data );
			}
			catch ( Exception ex )
			{
				Debug.LogException( ex );
			}
		}
		
		private bool InRange( Vector3 position )
		{
			var inX = position.x <= 1000 && position.x >= -1000;
			var inY = position.y <= 20 && position.y >= -20;
			var inZ = position.z <= 1000 && position.z >= -1000;

			return inX && inY && inZ;
		}

		private void OnValidate()
		{
			_collider = GetComponent<Collider>();
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void OnDrawGizmos()
		{
			if( _debugTrajectory == null)
				return;
			
			if ( _debugTrajectory.Count > 1 )
			{
				Gizmos.color = Color.red;
				
				for ( int i = 0; i < _debugTrajectory.Count - 1; i++ )
				{
					Gizmos.DrawLine( _debugTrajectory[i], _debugTrajectory[i + 1] );
				}
			}
		}

		public void Pause()
		{
			_isPaused = true;
		}

		public void Resume()
		{
			_isPaused = false;
		}
	}
}