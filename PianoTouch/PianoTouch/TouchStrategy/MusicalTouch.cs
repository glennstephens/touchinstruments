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
	}
}
