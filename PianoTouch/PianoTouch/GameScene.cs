using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using MediaPlayer;
using CoreMotion;
using TouchInstruments.Core;

namespace PianoTouch
{
	public enum NoteShape
	{
		LShape,
		BlackNote,
		MidNote,
		ReverseLShape
	}

	public class GameScene : SKScene
	{
		public GameScene (IntPtr handle) : base (handle)
		{
		}

		public override void DidMoveToView (SKView view)
		{
			CreatePianoKeys (5 * 12, 8);

			CreateOctaveKeys ();

			CreateInfoText ();
		}

		nfloat totalWidth;

		nfloat totalHeight;

		double keyWidth;

		string[] whiteKeyNames = new string[] { "C", "D", "E", "F", "G", "A", "B" };
		int[] placements = new int[] { 0, 1, 3, 4, 5 };
		string[] blackKeyNames = new string[] { "C#", "D#", "F#", "G#", "A#" };

		List<SKNode> pianoNoteNodes = new List<SKNode>();
		List<SKNode> whiteNotes = new List<SKNode>();
		List<SKNode> blackNotes = new List<SKNode>();

		NoteShape GetShapeForNote (int note)
		{
			switch (note % 12)
			{
				case 0: // C
					return NoteShape.LShape;
				case 1: // C#
					return NoteShape.BlackNote;
				case 2: // D
					return NoteShape.MidNote;
				case 3: // D#
					return NoteShape.BlackNote;
				case 4: // E
					return NoteShape.ReverseLShape;
				case 5: // F
					return NoteShape.LShape;
				case 6: // F#
					return NoteShape.BlackNote;
				case 7: // G
					return NoteShape.MidNote;
				case 8: // G#
					return NoteShape.BlackNote;
				case 9: // A
					return NoteShape.MidNote;
				case 10: // A#
					return NoteShape.BlackNote;
				case 11: // B
					return NoteShape.ReverseLShape;
				default:
					throw new ApplicationException ("There shouldn't be a note with this value");
			}
		}

		static List<string> allNoteNames = new List<string> {
			"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"	
		};

		string GetMusicalNameForNote(int note)
		{
			var noteName = allNoteNames [note % 12];
			var octave = note / 12;

			return string.Format ("{0}{1}", noteName, octave);
		}

		int GetMidiNoteForName(string name)
		{
			string note = name.Substring (0, name.Length - 1);
			int octave = Convert.ToInt32 (name.Substring (name.Length - 1, 1));
			return octave * 12 + allNoteNames.IndexOf (note);
		}

		int currentStartingNode;
		int notesPerPage;

		void CreatePianoKeys (int startingNote, int whiteNotesPerPage, bool useActualKeySize = false)
		{
			currentStartingNode = startingNote;
			notesPerPage = whiteNotesPerPage;

			RemoveChildren (pianoNoteNodes.ToArray ());
			pianoNoteNodes.Clear ();
			blackNotes.Clear ();
			whiteNotes.Clear ();

			nfloat topOffset = 0;

			totalWidth = this.Frame.Width;
			totalHeight = this.Frame.Height;

			if (!useActualKeySize)
				keyWidth = totalWidth / whiteNotesPerPage;
			else {
				// Get the actual size of the piano key in pixels based on the device
				var size = TouchInstruments.Core.iOSDimensions.GetCurrentDevice ();
				keyWidth = size.MillimetersToPixels (PianoDetails.WhiteKeyWidthInMMs);
			}

			int pos = 0;
			double currentX = 0;
			while (currentX <= whiteNotesPerPage)
			{
				var note = startingNote + pos;
				var noteName = GetMusicalNameForNote (note);
				NoteShape shape = GetShapeForNote (note);
				Console.WriteLine (noteName + ": " + shape);

				// Add the note
				SKShapeNode noteNode = null;
				switch (shape)
				{
					case NoteShape.LShape:
						noteNode = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth, totalHeight)));
						noteNode.Position = new CGPoint(currentX * keyWidth, topOffset);
						noteNode.FillColor = UIColor.White;
						noteNode.StrokeColor = UIColor.LightGray;
						break;
					case NoteShape.BlackNote:
						noteNode = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), 
							new CGSize (keyWidth / 2, totalHeight / 2)));

						noteNode.Position = new CGPoint (currentX * keyWidth - keyWidth / 2 + keyWidth / 4, 
							topOffset + totalHeight / 2);

						noteNode.FillColor = UIColor.Black;
						noteNode.StrokeColor = UIColor.DarkGray;
						break;
					case NoteShape.MidNote:
						noteNode = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth, totalHeight)));
						noteNode.Position = new CGPoint(currentX * keyWidth, topOffset);
						noteNode.FillColor = UIColor.White;
						noteNode.StrokeColor = UIColor.LightGray;
						break;
					case NoteShape.ReverseLShape:
						noteNode = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth, totalHeight)));
						noteNode.Position = new CGPoint(currentX * keyWidth, topOffset);
						noteNode.FillColor = UIColor.White;
						noteNode.StrokeColor = UIColor.LightGray;
						break;
				}
				noteNode.Name = GetMusicalNameForNote (note);

				pianoNoteNodes.Add (noteNode);

				if (shape != NoteShape.BlackNote) {
					currentX++;
					whiteNotes.Add (noteNode);
				} else {
					blackNotes.Add (noteNode);
				}

				pos++;
			}

			whiteNotes.ForEach (AddChild);
			blackNotes.ForEach (AddChild);
		}

		void CreateButtonAtPoint(string name, string buttonText, CGSize size, CGPoint viewPoint)
		{
			var buttonBackground = SKShapeNode.FromRect (size);
			buttonBackground.FillColor = UIColor.FromRGB (222, 222, 222);
			buttonBackground.StrokeColor = UIColor.FromRGB (211, 211, 211);
			buttonBackground.Position = Scene.ConvertPointFromView (viewPoint);
			buttonBackground.Name = name;

			AddChild (buttonBackground);

			var buttonTextNode = new SKLabelNode (defaultFontName) {
				Text = buttonText,
				FontSize = 33,
				FontColor = UIColor.Black,
			};

			CGPoint newPoint = new CGPoint (viewPoint.X, viewPoint.Y + 8);
			buttonTextNode.Position = Scene.ConvertPointFromView (newPoint);

			AddChild (buttonTextNode);
		}

		void CreateOctaveKeys ()
		{
//			// Get the rectangle
//			var barTopLeft = this.Scene.ConvertPointFromView (new CGPoint(0, 0));
//			var barBottomRight = this.Scene.ConvertPointFromView (new CGPoint(UIScreen.MainScreen.Bounds.Width, 40));
//
//			var mainToolbarBackground = SKShapeNode.FromRect (
//				new CGRect(barTopLeft.X, barTopLeft.Y, barBottomRight.X - barTopLeft.X, barBottomRight.Y - barTopLeft.Y));
//			mainToolbarBackground.FillColor = UIColor.FromRGB (222, 222, 222);
//			mainToolbarBackground.StrokeColor = UIColor.FromRGB (211, 211, 211);

			CreateButtonAtPoint ("Control-7", "-7", new CGSize (80, 80), new CGPoint (40, 24));
			CreateButtonAtPoint ("Control-1", "-1", new CGSize (80, 80), new CGPoint (85, 24));
			CreateButtonAtPoint ("Control+1", "+1", new CGSize (80, 80), new CGPoint (130, 24));
			CreateButtonAtPoint ("Control+7", "+7", new CGSize (80, 80), new CGPoint (175, 24));
		}

		string defaultFontName = "Avenir-Light";

		SKLabelNode motionManagerNode;

		void CreateInfoText ()
		{
//			motionManagerNode = new SKLabelNode (defaultFontName) {
//				Text = "Piano",
//				FontSize = 17,
//				FontColor = UIColor.Black,
//				Position = new CGPoint (80, Frame.Height / 6)
//			};
//
//			AddChild (motionManagerNode);
		}

		List<string> noteNames = new List<string>(
			new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"});

		int currentLevel = 127;

		MusicalTouches allTouches = 
			new MusicalTouches(GameViewController.MidiControl, 
				new OSVersionCheckStrategy());

		SKNode GetMusicalNodeAtLocation (CGPoint location)
		{
			var allNodes = GetNodesAtPoint (location)
				.Where (n => !String.IsNullOrEmpty (n.Name));
			var blackNode = allNodes.FirstOrDefault (n => n.Name.Contains ("#"));
			if (blackNode != null)
				return blackNode;
			else
				return allNodes.FirstOrDefault (n => !n.Name.Contains ("#"));
		}

		bool LookForControlMessage (CGPoint location)
		{
			var controlNode = GetNodesAtPoint (location)
				.FirstOrDefault (n => !String.IsNullOrEmpty (n.Name) && n.Name.StartsWith ("Control"));

			if (controlNode != null)
			{
				var amount = Convert.ToInt32 (controlNode.Name.Substring (7));

				// Recreate the display
				var newPosition = currentStartingNode + amount;

				CreatePianoKeys (newPosition, notesPerPage);

				return true;
			}

			return false;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (UITouch touch in touches) {

				// Check For a Control Message
				var loc = touch.LocationInNode (this);
				var didProcessControlMessage = LookForControlMessage (loc);
				if (didProcessControlMessage)
					continue;

				var node = GetMusicalNodeAtLocation (loc);

				if (node != null)
				{
					var index = GetMidiNoteForName (node.Name);
					if (index >= 0)
					{
						var musicTouch = allTouches.StartNote (touch, index);
						PlayFadeOutNoteDisplay (node);
					}
				}
			}
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			foreach (UITouch touch in touches) {
				var loc = touch.LocationInNode (this);

				var didProcessControlMessage = LookForControlMessage (loc);
				if (didProcessControlMessage)
					continue;
				
				var node = GetMusicalNodeAtLocation (loc);

				if (node != null) {
					var index = GetMidiNoteForName (node.Name);
					if (index >= 0) {
						var mn = allTouches.UpdateNote (touch, index);

						if (mn.DidChangeVisuals)
							PlayFadeOutNoteDisplay (node);
					}
				}
			}
		}

		void PlayFadeOutNoteDisplay (SKNode node)
		{
			// Play the fade out note
			var note = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), node.Frame.Size));
			note.Position = node.Position;
			note.FillColor = UIColor.Blue;
			note.StrokeColor = UIColor.Blue;
			note.RunAction (SKAction.Group (SKAction.FadeOutWithDuration (0.35f)), () => RemoveChildren (new SKNode[] {
				note
			}));

			AddChild (note);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			foreach (UITouch touch in touches) {
				var loc = touch.LocationInNode (this);

				var didProcessControlMessage = LookForControlMessage (loc);
				if (didProcessControlMessage)
					continue;
				
				var node = GetMusicalNodeAtLocation (touch.LocationInNode (this));

				if (node != null) {
					var index = GetMidiNoteForName (node.Name);
					if (index >= 0) {
						allTouches.EndNote (touch, index);
					}
				}
			}
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
		}

		public override void Update (double currentTime)
		{
			// Called before each frame is rendered
		}
	}
}

