namespace _Game._Scripts.Level
{
	using UnityEngine;

	public class SystemPauseService
	{
		private int _pauseType;

		public SystemPauseService()
		{
		}

		public void ToggleTimescalePause()
		{
			_pauseType = (_pauseType + 1) % 3;
			float value = 1f;

			switch ( _pauseType )
			{
				case 0:
					value = 1;
					break;
				case 1:
					value = 0.1f;
					break;
				case 2:
					value = 0f;
					break;
			}

			Time.timeScale = value;
		}
	}
}