namespace _Game._Scripts.Level
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using _Game._Scripts.Character.Enemy;
	using _Game._Scripts.Character.Hero;
	using _Game._Scripts.Enums;
	using _Game._Scripts.Interfaces;
	using Sirenix.OdinInspector;
	using UniRx;
	using UnityEngine;
	using Zenject;

	[Serializable]
	public struct TragetDistanceRecord
	{
		public ITarget Target;
		public string Name;
		public float Distance;
	}
	
	public class LevelTargetsProvider: MonoBehaviour, IInitializable, ITickable
	{
		[HideInInspector] public int ID;
		[SerializeField, ReadOnly] public string _nearestAnyLiveEnemyName;
		[SerializeField, ReadOnly] public string _nearestLiveEnemyInRangeName;
		[SerializeField, ReadOnly] public string _nearestAnyTargetName;
		
		[SerializeField, ReadOnly, Space] private List<TragetDistanceRecord> _targetDistanceRecords;

		private List<ITarget> _targets;
		private IHeroFacade[] _heroFacades;
		private IEnemyFacade[] _enemyFacades;
		private List<TragetDistanceRecord> _aggressiveEnemiesRecords;
		private IAreaTarget[] _areaTargets;

		private CompositeDisposable _targetsDisposable;

		public int HeroTargetsCount => _heroFacades.Length;
		
		public IHeroFacade MainHeroFacade { get; private set; }
		public IHeroFacade LiveHeroFacade => MainHeroFacade == null ? null : (MainHeroFacade.IsDead ? null : MainHeroFacade);

		public IEnumerable<IEnemyFacade> EnemyTargets => _enemyFacades;
		public IEnumerable<IAreaTarget> AreaTargets => _areaTargets;


		public ReactiveProperty<IEnemyFacade> NearestLiveEnemy { get; } = new ReactiveProperty<IEnemyFacade>();
		public ReactiveProperty<IEnemyFacade> NearestLiveEnemyInAttackRange { get; } = new ReactiveProperty<IEnemyFacade>();
		public ReactiveProperty<IEnemyFacade> NearestAggressiveEnemy { get; } = new ReactiveProperty<IEnemyFacade>();
		public ReactiveProperty<ITarget> NearestAnyTargetInRange { get; } = new ReactiveProperty<ITarget>();
		public ReactiveProperty<int> AggressiveEnemiesCount { get; } = new ReactiveProperty<int>();


		[Inject]
		public void Construct( List<ITarget> targets )
		{
			_targets = targets;
			_targetsDisposable = new CompositeDisposable();

			_targetDistanceRecords = new List<TragetDistanceRecord>(_targets.Count);
			_aggressiveEnemiesRecords = new List<TragetDistanceRecord>( _targets.Count );
		}

		public void Initialize()
		{
			InitializeInternal();
		}

		private void InitializeInternal()
		{
			InitializeOfInitializables();
			UpdateTypedTargets();

			Debug.Log( $"LevelTargetsProvider {ID} Construct heroes {_heroFacades.Length}, enemies {_enemyFacades.Length}," );
		}

		private void InitializeOfInitializables()
		{
			foreach ( var target in _targets )
			{
				if ( target is IInitializable initializable )
				{
					initializable.Initialize();
				}
			}
		}


		public void Tick()
		{
			if( _heroFacades.Length == 0)
				return;

			var heroPosition = MainHeroFacade.Position;
			
			ITarget nearestTarget = null;
			IEnemyFacade nearestEnemyInAttackRange = null;

			UpdateSqrDistancesFrom( heroPosition );
			UpdateNearestAggressiveEnemy();

			NearestLiveEnemy.Value = (IEnemyFacade)FindNearestTargetsFromHero( ETargetFilter.LiveEnemy );

			if ( NearestLiveEnemy.Value != null )
			{
				var attackDistance = MainHeroFacade.Equip.WeaponAttackDistance;
				var inRange = Vector3.SqrMagnitude( heroPosition - NearestLiveEnemy.Value.Position ) <= attackDistance * attackDistance;
				nearestEnemyInAttackRange = inRange ? NearestLiveEnemy.Value : null;
			}

			NearestLiveEnemyInAttackRange.Value = nearestEnemyInAttackRange;

			nearestTarget = SelectNearest( nearestTarget, nearestEnemyInAttackRange, heroPosition );
			NearestAnyTargetInRange.Value = nearestTarget;

			_nearestAnyLiveEnemyName = NearestLiveEnemy.Value == null ? "" : NearestLiveEnemy.Value.Name;
			_nearestLiveEnemyInRangeName = NearestLiveEnemyInAttackRange.Value == null ? "" : NearestLiveEnemyInAttackRange.Value.Name;
			_nearestAnyTargetName = NearestAnyTargetInRange.Value == null ? "" : NearestAnyTargetInRange.Value.Name;
		}

		private void UpdateSqrDistancesFrom( Vector3 heroPosition )
		{
			_targetDistanceRecords.Clear();
			
			for ( var i = 0; i < _targets.Count; i++ )
			{
				var target = _targets[i];

				if ( target is ILifeCycleTarget { IsDestroyed: true } )
				{
					continue;
				}

				var record = new TragetDistanceRecord()
				{
					Target = target,
					Name = target.Name,
					Distance = Vector3.SqrMagnitude( target.GetClosestPoint( heroPosition ) - heroPosition )
				};

				_targetDistanceRecords.Add(record);
			}
			
			_targetDistanceRecords.Sort( CompareDistance() );
		}

		private Comparison<TragetDistanceRecord> CompareDistance()
		{
			return ( r1, r2 ) =>
			{
				if ( r1.Distance < r2.Distance )
					return -1;
				
				if ( r1.Distance > r2.Distance )
					return 1;
				
				return 0;
			};
		}

		private void UpdateNearestAggressiveEnemy()
		{
			_aggressiveEnemiesRecords.Clear();
			
			TargetConditions.Hero = MainHeroFacade;
			
			foreach ( var record in _targetDistanceRecords )
			{
				if(IsTargetStatifiesCondition( record.Target, ETargetFilter.LiveAggressiveEnemy ))
					_aggressiveEnemiesRecords.Add( record );
			}

			AggressiveEnemiesCount.Value = _aggressiveEnemiesRecords.Count;
			NearestAggressiveEnemy.Value = _aggressiveEnemiesRecords.Count > 0 ? _aggressiveEnemiesRecords[0].Target as IEnemyFacade : null;
		}

		public void AddTarget(ITarget target)
		{
			if ( _targets.Contains( target ) )
			{
				Debug.LogWarning( $"Target {target.Name} contains already" );
				return;
			}

			_targets.Add( target );
			_targetDistanceRecords = new List<TragetDistanceRecord>( _targets.Count );
			_aggressiveEnemiesRecords = new List<TragetDistanceRecord>( _targets.Count );
			
			UpdateTypedTargets();

            MainHeroFacade = _heroFacades[0];
        }

		public void RemoveTarget( ITarget target )
		{
			if ( _targets.Contains( target ) == false )
			{
				Debug.LogWarning( $"Target {target.Name} no contains" );
				return;
			}

			_targets.Remove( target );

			UpdateTypedTargets();
		}

		public void RemoveTarget( int index )
		{
			_targets.RemoveAt( index );

			UpdateTypedTargets();
		}
		
		public ITarget FindNearestTargetsFromHero(ETargetFilter typeFilter)
		{
			if ( _targetDistanceRecords.Count == 0 )
				return null;
			
			ITarget target = null;
			TargetConditions.Hero = MainHeroFacade;

			for ( var i = 0; i < _targetDistanceRecords.Count; i++ )
			{
				var record = _targetDistanceRecords[i];
				var recordTarget = record.Target;

				if ( IsTargetStatifiesCondition(recordTarget, typeFilter) )
				{
					target = recordTarget;
					break;
				}
			}
			
			return target;
		}

		public ITarget SelectNearest( ITarget firstTarget, ITarget secondTarget, Vector3 basePosition )
		{
			if ( firstTarget == null && secondTarget == null )
				return null;

			if ( firstTarget == null )
				return secondTarget;

			if ( secondTarget == null )
				return firstTarget;

			return IsFirstTargetNearestThanSecond( firstTarget, secondTarget, basePosition ) ? firstTarget : secondTarget;
		}

		private bool IsFirstTargetNearestThanSecond( ITarget firstTarget, ITarget secondTarget, Vector3 basePosition )
		{
			var firstSqrDistance = Vector3.SqrMagnitude( firstTarget.Position - basePosition );
			var secondSqrDistance = Vector3.SqrMagnitude( secondTarget.Position - basePosition );

			return firstSqrDistance < secondSqrDistance;
		}

		private void UpdateTypedTargets()
		{
			_heroFacades = _targets.OfType<IHeroFacade>().ToArray();
			_enemyFacades = _targets.OfType<IEnemyFacade>().ToArray();
			_areaTargets = _targets.OfType<IAreaTarget>().ToArray();
		}

		private void UnsubscribeTargets()
		{
			_targetsDisposable.Clear();
		}

		private bool IsTargetStatifiesCondition( ITarget target, ETargetFilter typeFilter ) 
		{
			Func<ITarget, bool> condition;

			switch ( typeFilter )
			{
				case ETargetFilter.Any:    
					return TargetConditions.IsAny( target );
				case ETargetFilter.AnyHero: 
					return TargetConditions.IsAnyHero( target );
				case ETargetFilter.LiveHero:
					return TargetConditions.IsLiveHero( target );
				case ETargetFilter.DeadHero:
					return TargetConditions.IsDeadHero( target );
				case ETargetFilter.AnyEnemy:
					return TargetConditions.IsAnyEnemy( target );
				case ETargetFilter.LiveEnemy:
					return TargetConditions.IsLiveEnemy( target );
				case ETargetFilter.LiveAggressiveEnemy:
					return TargetConditions.IsAggressiveEnemy( target );
				case ETargetFilter.DeadEnemy:
					return TargetConditions.IsDeadEnemy( target );

				default:
					throw new ArgumentOutOfRangeException( nameof(typeFilter), typeFilter, null );
			}
		}
	}
}