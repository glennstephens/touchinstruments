using System;
using UIKit;
using System.Collections.Generic;

namespace PianoTouch
{
	public class MusicalTouch
	{
		public int Note { get; set; }

		public UITouch Touch { get; set; }

		public int Volume { get; set; }

		public bool DidChangeVisuals { get; set; }

		public DateTime FirstTouchTime { get; set; }

		public double InitialForcePercentage { get; set; }

		public void AddVolume(int v)
		{
			AccumulatedVolume += v;
			AccumulatedVolumeCount++;
		}

		int AccumulatedVolume { get; set; }

		int AccumulatedVolumeCount { get; set; }

		public int CalculatedVolume { 
			get {
				if (AccumulatedVolumeCount == 0)
					return 0;
				else
					return AccumulatedVolume / AccumulatedVolumeCount;
			}
		}

	}
}
