using System;
using UIKit;
using System.Collections.Generic;
using PianoTouch;
using CoreImage;

namespace PianoTouch
{
	public abstract class BaseTouchStrategy
	{
		public abstract int TouchesDown (UITouch touch);
		public abstract int TouchesMove (UITouch touch);
		public abstract int TouchesUp (UITouch touch);
	}

	public class ConstantVolumeTouchStrategy : BaseTouchStrategy 
	{
		int constantVolume;

		public ConstantVolumeTouchStrategy(int constantVolume)
		{
			this.constantVolume = constantVolume;
		}

		public override int TouchesDown (UITouch touch) 
		{
			return constantVolume;
		}

		public override int TouchesMove (UITouch touch)
		{
			return constantVolume;
		}

		public override int TouchesUp (UITouch touch)
		{
			return constantVolume;
		}
	}

	public class ForcePercentageTouchStrategy : BaseTouchStrategy
	{
		protected virtual int PressureForTouch(UITouch touch)
		{
			if (touch.MaximumPossibleForce != 0)
				return Convert.ToInt32 (touch.Force / touch.MaximumPossibleForce * 127);	

			return 127;
		}

		public override int TouchesDown (UITouch touch) 
		{
			return PressureForTouch(touch);
		}

		public override int TouchesMove (UITouch touch)
		{
			return PressureForTouch(touch);
		}

		public override int TouchesUp (UITouch touch)
		{
			return PressureForTouch(touch);
		}
	}

	public class ExonentialForceTouchStrategy : ForcePercentageTouchStrategy
	{
		protected override int PressureForTouch(UITouch touch)
		{
			// This one we are going to use an exponential function
			if (touch.MaximumPossibleForce == 0)
				return 127;

			var percentage = touch.Force / touch.MaximumPossibleForce * 4;	

			// We need an exponential value between 1 and 127
			var volume = 64.0 * Math.Pow (Math.E, percentage);
			if (volume > 127)
				volume = 127;

			return Convert.ToInt32 (volume);
		}
	}
}

