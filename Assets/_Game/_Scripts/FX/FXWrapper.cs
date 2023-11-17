namespace _Game._Scripts.FX
{
	using UnityEngine;

	public class FXWrapper : MonoBehaviour
	{
		[SerializeField] private ParticleSystem PS;
		[SerializeField] private ParticleSystem TrailPS;

		private Transform _fxTransform;
		private Transform _wrapperTransform;
		private ParticleSystemRenderer _psRenderer;
		private ParticleSystem.MainModule _psMain;
		private ParticleSystem.EmissionModule _psEmission;
		private bool _isCached;
		
		private TrailRenderer[] _trails;

		public bool IsAlive => PS.IsAlive();
		public float LifeTime => PS.main.duration;
		public float TrailLifeTime => TrailPS != null ? TrailPS.main.duration : 0;

		private void Awake()
		{
			_trails = GetComponentsInChildren<TrailRenderer>();
		}


		public void Play()
		{
			if ( !PS.isPlaying )
				PS.Play( true );
		}

		public void PlayProperly( bool withChildren = true )
		{
			Debug.Log( $"FX [{name}] : PlayProperly" );
			
			Stop();

			PS.Play( withChildren );
		}

		public void Stop()
		{
			if ( PS.isPlaying )
			{
				PS.Stop( true );
			}

			foreach ( var trail in _trails )
			{
				try
				{
					trail.Clear();
				}
				catch ( MissingReferenceException ex )
				{
					Debug.LogError( $"Missed trail on {transform.parent.name}" );
				}
			}
		}

		public void Pause()
		{
			if ( !PS.isPaused )
				PS.Pause(true);
		}

		public void SetPosition( Vector3 position )
		{
			_wrapperTransform.position = position;
		}

		public void SetRotation( Quaternion rotation )
		{
			_wrapperTransform.rotation = rotation;
		}

		public void Show()
		{
			_fxTransform.gameObject.SetActive( true );
		}

		public void Hide()
		{
			_fxTransform.gameObject.SetActive( false );
		}

		public void CacheValues()
		{
			if ( _isCached )
				return;

			CacheValuesInternal();

			_isCached = true;
		}

		protected virtual void CacheValuesInternal()
		{
			_wrapperTransform = transform;
			_fxTransform = PS.transform;
			_psMain = PS.main;
			_psEmission = PS.emission;
			_psRenderer = PS.GetComponent<ParticleSystemRenderer>();
		}
	}
}

