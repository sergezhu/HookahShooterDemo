namespace _Game._Scripts.Managers
{
	public interface ITapticController
	{
		void DoLight();
		void DoMedium();
		void DoHeavy();
	}
	
	public class TapticController : ITapticController
	{
		public void DoLight()
		{
			AndroidTaptic.Haptic( HapticTypes.LightImpact );
		}

		public void DoMedium()
		{
			AndroidTaptic.Haptic( HapticTypes.MediumImpact );
		}

		public void DoHeavy()
		{
			AndroidTaptic.Haptic( HapticTypes.HeavyImpact );
		}
	}
}