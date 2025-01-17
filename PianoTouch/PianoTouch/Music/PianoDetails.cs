﻿using System;

namespace PianoTouch
{
	public class PianoDetails
	{
		// Based on the fact that an octave is 48-48.5 inches
		// Octave width is 164-165 mm
		// Seven keys in that distance
		// See: https://en.wikipedia.org/wiki/Musical_keyboard for more info

		// Further information is available from: http://www.quadibloc.com/other/cnv05.htm

		public static double OctaveSizeMM = 165.0;

		public static double WhiteKeyCount = 7;
		public static double BlackKeyCount = 5;

		public static double WhiteKeyWidthInMMs = 165.0 / 7.0;
	}
}

