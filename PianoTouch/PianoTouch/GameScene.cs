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

		void CreatePianoKeys (int startingNote, int whiteNotesPerPage)
		{
			RemoveChildren (pianoNoteNodes.ToArray ());
			pianoNoteNodes.Clear ();
			blackNotes.Clear ();
			whiteNotes.Clear ();

			totalWidth = this.Frame.Width;
			totalHeight = this.Frame.Height;

			keyWidth = totalWidth / whiteNotesPerPage;

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
						noteNode.Position = new CGPoint(currentX * keyWidth, 0);
						noteNode.FillColor = UIColor.White;
						noteNode.StrokeColor = UIColor.LightGray;
						break;
					case NoteShape.BlackNote:
						noteNode = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth / 2, totalHeight / 2)));
						noteNode.Position = new CGPoint (currentX * keyWidth - keyWidth / 2 + keyWidth / 4, totalHeight / 2);
						noteNode.FillColor = UIColor.Black;
						noteNode.StrokeColor = UIColor.DarkGray;
						break;
					case NoteShape.MidNote:
						noteNode = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth, totalHeight)));
						noteNode.Position = new CGPoint(currentX * keyWidth, 0);
						noteNode.FillColor = UIColor.White;
						noteNode.StrokeColor = UIColor.LightGray;
						break;
					case NoteShape.ReverseLShape:
						noteNode = SKShapeNode.FromRect (new CGRect (new CGPoint (0, 0), new CGSize (keyWidth, totalHeight)));
						noteNode.Position = new CGPoint(currentX * keyWidth, 0);
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

		void CreateOctaveKeys ()
		{
			
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
				new ExonentialForceTouchStrategy());

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (UITouch touch in touches) {
				var location = touch.LocationInNode (this);
				var node = GetNodesAtPoint (location)
					.FirstOrDefault (n => !String.IsNullOrEmpty (n.Name));

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
				var location = touch.LocationInNode (this);
				var node = GetNodesAtPoint (location)
					.FirstOrDefault (n => !String.IsNullOrEmpty (n.Name));

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
				var location = touch.LocationInNode (this);
				var node = GetNodesAtPoint (location)
					.FirstOrDefault (n => !String.IsNullOrEmpty (n.Name));

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

