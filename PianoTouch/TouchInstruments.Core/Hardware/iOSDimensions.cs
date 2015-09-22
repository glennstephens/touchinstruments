using System;
using UIKit;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace TouchInstruments.Core
{
	public class PhoneSize
	{
		int pixelsWidth;

		public int PixelsWidth {
			get {
				return pixelsWidth;
			}
			set {
				pixelsWidth = value;
			}
		}

		int pixelsHeight;

		public int PixelsHeight {
			get {
				return pixelsHeight;
			}
			set {
				pixelsHeight = value;
			}
		}

		double diagonalWidth;

		public double DiagonalWidth {
			get {
				return diagonalWidth;
			}
			set {
				diagonalWidth = value;
			}
		}

		double heightInInches;

		public double HeightInInches {
			get {
				return heightInInches;
			}
			set {
				heightInInches = value;
			}
		}

		double widthInInches;

		public double WidthInInches {
			get {
				return widthInInches;
			}
			set {
				widthInInches = value;
			}
		}

		double pixelsToInchesRatio;

		public double PixelsToInchesRatio {
			get {
				return pixelsToInchesRatio;
			}
			set {
				pixelsToInchesRatio = value;
			}
		}

		public PhoneSize(int pixelsWidth, int pixelsHeight, double diagonalScreenSizeInInches)
		{
			this.diagonalWidth = diagonalScreenSizeInInches;
			this.pixelsHeight = pixelsHeight;
			this.pixelsWidth = pixelsWidth;

			// Calculate the smallest size

			// Get the hypotonuse
			var screenDiagnalInPixels = Math.Sqrt (pixelsWidth * pixelsWidth + pixelsHeight * pixelsHeight);
			pixelsToInchesRatio = screenDiagnalInPixels / diagonalScreenSizeInInches;

			heightInInches = pixelsHeight / pixelsToInchesRatio;
			widthInInches = pixelsWidth / pixelsToInchesRatio;
		}

		public double InchesToPixels(double inches)
		{
			return inches / pixelsToInchesRatio;
		}

		public double MillimetersToPixels(double mms)
		{
			return InchesToPixels (mms * 25.4);
		}
	}

	// Details taken from http://www.iosres.com/
	public class iOSDimensions
	{
		public static PhoneSize iPhone1to4Sizes = new PhoneSize(320, 480, 3.5);
		public static PhoneSize iPhone5Sizes = new PhoneSize(320, 568, 4.0);
		public static PhoneSize iPhone6Sizes = new PhoneSize(375, 667, 4.7);
		public static PhoneSize iPhone6PlusSizes = new PhoneSize(414, 736, 5.5);
	}
}
