using System;
using UIKit;
using System.Collections.Generic;
using PianoTouch;
using CoreImage;
using System.Collections.Specialized;

namespace PianoTouch
{
	public abstract class BaseTouchStrategy
	{
		public abstract int TouchesDown (UITouch touch);
		public abstract int TouchesMove (UITouch touch);
		public abstract int TouchesUp (UITouch touch);

		public abstract bool UseInitialTouch { get; }
		public abstract int MillisecondsFromFirstTouch { get; }
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

		public override bool UseInitialTouch {
			get {
				return true;
			}
		}

		public override int MillisecondsFromFirstTouch {
			get {
				return 0;
			}
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

		public override bool UseInitialTouch {
			get {
				return false;
			}
		}

		// Defines the latency for the system
		public override int MillisecondsFromFirstTouch {
			get {
				return 100;
			}
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
			var volume = 52.0 * Math.Pow (Math.E, percentage);
			if (volume > 127)
				volume = 127;

			return Convert.ToInt32 (volume);
		}
	}

	public class OSVersionCheckStrategy : BaseTouchStrategy
	{
		ExonentialForceTouchStrategy force = new ExonentialForceTouchStrategy ();
		ConstantVolumeTouchStrategy constant = new ConstantVolumeTouchStrategy (64);

		BaseTouchStrategy strategy;

		public OSVersionCheckStrategy() : base()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				strategy = force;
			} else
				strategy = constant;
		}

		public override int TouchesDown (UITouch touch)
		{
			return strategy.TouchesDown (touch);
		}

		public override int TouchesMove (UITouch touch)
		{
			return strategy.TouchesMove (touch);
		}

		public override int TouchesUp (UITouch touch)
		{
			return strategy.TouchesUp (touch);
		}

		public override bool UseInitialTouch {
			get {
				return false;
			}
		}

		public override int MillisecondsFromFirstTouch {
			get {
				return 75;
			}
		}
	}
}

