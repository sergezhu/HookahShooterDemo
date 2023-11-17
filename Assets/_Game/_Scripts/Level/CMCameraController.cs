namespace _Game._Scripts.Level
{
	using System.Collections;
	using _Game._Scripts.Utilities;
	using _Game._Scripts.Utilities.Extensions;
	using Cinemachine;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class CMCameraController : MonoBehaviour, IInitializable
	{
		
		[SerializeField] private CinemachineVirtualCamera _levelCamera;
		[SerializeField] private CinemachineVirtualCamera _heroCamera;

		[Inject] private CinemachineBrain _brain;
		[Inject] private ScreenResizeDetector _screenResizeDetector;

		private Camera _camera;
		private CinemachineFramingTransposer _levelCameraTransposer;
		private CinemachineFramingTransposer _heroCameraTransposer;

		public void Initialize()
		{
			_camera = Camera.main;
			_levelCameraTransposer = _levelCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
			_heroCameraTransposer = _heroCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

			_levelCamera.Priority = 10;
			_heroCamera.Priority = 1;

			_screenResizeDetector.Size
				.Subscribe( size => OnScreenResized( size ) )
				.AddTo( this );
		}

		public void SetHeroCamerasTarget( Transform target )
		{
			_brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
			_heroCamera.Follow = target;

			StartCoroutine( SetHeroTargetRoutine() );
		}


		private IEnumerator SetHeroTargetRoutine()
		{
			Vector3 cachedDamping = new Vector3( _heroCameraTransposer.m_XDamping, _heroCameraTransposer.m_YDamping, _heroCameraTransposer.m_ZDamping );
			
			_heroCameraTransposer.m_XDamping = 0;
			_heroCameraTransposer.m_YDamping = 0;
			_heroCameraTransposer.m_ZDamping = 0;

			yield return new WaitForSeconds( 0.01f );

			_heroCameraTransposer.m_XDamping = cachedDamping.x;
			_heroCameraTransposer.m_YDamping = cachedDamping.y;
			_heroCameraTransposer.m_ZDamping = cachedDamping.z;
		}

		public void SwitchToDefaultCamera() => SwitchTo( _levelCamera );
		public void SwitchToHeroCamera() => SwitchTo( _heroCamera );

		private void SwitchTo( CinemachineVirtualCamera cam )
		{
			var cameras = new[] { _levelCamera, _heroCamera };
			cameras.ForEach( c => c.Priority = c == cam ? 10 : 1 );
		}

		private void OnScreenResized( Vector2Int size )
		{
			Debug.Log( $"OnResize : {size}" );
		}

		public Vector3 GetPointAtHeight( float screenX, float screenY, float height )
		{
			return _camera.GetPointAtHeightFromRelative( screenX, screenY, height );
		}

		public Vector2 ForwardXZ => _camera.transform.forward.xz().normalized;
		public float RotationAngleY => _camera.transform.rotation.eulerAngles.y;

		public Vector3 GetHeroCameraRotation()
		{
			return _heroCamera.transform.rotation.eulerAngles;
		}

		public void SetHeroCameraRotation(Vector3 euler)
		{
			_heroCamera.transform.rotation = Quaternion.Euler( euler );
			Debug.Log( $"SetHeroCameraRotation : {euler}" );
		}
	}
}

