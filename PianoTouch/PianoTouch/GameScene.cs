using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using MediaPlayer;
using CoreMotion;

namespace PianoTouch
{
	public class GameScene : SKScene
	{
		public GameScene (IntPtr handle) : base (handle)
		{
		}

		public override void DidMoveToView (SKView view)
		{
			// Create the Child Views
			CreatePianoKeys ();

			CreateOctaveKeys ();

			CreateInfoText ();

			// Setup your scene here
			//SetupAccelerometer ();
		}

		nfloat totalWidth;

		nfloat totalHeight;

		double keyWidth;

		string[] whiteKeyNames = new string[] { "C", "D", "E", "F", "G", "A", "B" };
		int[] placements = new int[] { 0, 1, 3, 4, 5 };
		string[] blackKeyNames = new string[] { "C#", "D#", "F#", "G#", "A#" };

		void CreatePianoKeys ()
		{
			totalWidth = this.Frame.Width;
			totalHeight = this.Frame.Height;

			keyWidth = totalWidth / TouchInstruments.Core.PianoDetails.WhiteKeyCount;

			// Add the White Keys


			for (int i=0; i < TouchInstruments.Core.PianoDetails.WhiteKeyCount; i++)
			{
				var note = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth, totalHeight)));
				note.Position = new CGPoint(i * keyWidth, 0);
				note.FillColor = UIColor.White;
				note.StrokeColor = UIColor.LightGray;
				note.Name = whiteKeyNames [i];

				AddChild (note);
			}

			// Add the Black Keys
			for (int i=0; i < placements.Length; i++)
			{
				var note = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth / 2, totalHeight / 2)));
				note.Position = new CGPoint (placements [i] * keyWidth + keyWidth / 2 + keyWidth / 4, totalHeight / 2);
				note.FillColor = UIColor.Black;
				note.StrokeColor = UIColor.DarkGray;
				note.Name = blackKeyNames [i];

				AddChild (note);
			}
		}

		void CreateOctaveKeys ()
		{
			
		}

		string defaultFontName = "Avenir-Light";

		SKLabelNode motionManagerNode;

		void CreateInfoText ()
		{
			motionManagerNode = new SKLabelNode (defaultFontName) {
				Text = "Piano",
				FontSize = 17,
				FontColor = UIColor.Black,
				Position = new CGPoint (80, Frame.Height / 6)
			};

			AddChild (motionManagerNode);
		}

		List<string> noteNames = new List<string>(
			new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"});

		int currentLevel = 127;

		void SetupAccelerometer ()
		{
			var motionManager = new CMMotionManager ();
			motionManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, error) => {
				if (error == null)
				{
					// 1 is when the app is vertical
					currentLevel = Convert.ToInt32(data.Acceleration.X / 2f * 256f);
					Console.WriteLine (data.Acceleration.X);
				}
			});
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			// Called when a touch begins
			foreach (UITouch touch in touches) {
				var location = touch.LocationInNode (this);
				var node = GetNodesAtPoint (location)
					.LastOrDefault (n => !String.IsNullOrEmpty (n.Name));

				if (node != null)
				{
					var baseTone = 5 * 12 - 2;

					// Get the index
					var index = noteNames.IndexOf (node.Name);
					if (index >= 0)
					{
						// Get the pressure
						int amount = 127;
						if (touch.MaximumPossibleForce != 0)
							amount = Convert.ToInt32 (touch.Force / touch.MaximumPossibleForce * 256);

						GameViewController.MidiControl.PlayNote (baseTone + index, 1, amount);

						var clickedNote = node as SKShapeNode;
						var note = SKShapeNode.FromRect (new CGRect(new CGPoint(0, 0), node.Frame.Size));
						note.Position = node.Position;
						note.FillColor = UIColor.Blue;
						note.StrokeColor = UIColor.Blue;

						note.RunAction (SKAction.Group (
							//SKAction.ScaleBy (1.4f, 0.3f),
							SKAction.FadeOutWithDuration (0.35f)
						), () => RemoveChildren (new SKNode[] { note }));
						
						AddChild (note);
					}
				}
			}
		}

		public override void Update (double currentTime)
		{
			// Called before each frame is rendered
		}
	}
}

