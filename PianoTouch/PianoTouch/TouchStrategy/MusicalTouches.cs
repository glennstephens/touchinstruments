using System;
using UIKit;
using System.Collections.Generic;
using TouchInstruments.Core;
using System.Threading.Tasks;

namespace PianoTouch
{
	public class MusicalTouches
	{
		BaseTouchStrategy volumeStrategy;

		MidiControl midi;

		public MusicalTouches(MidiControl midi, BaseTouchStrategy volumeStrategy)
		{
			this.midi = midi;
			this.volumeStrategy = volumeStrategy;
		}

		public Dictionary<UITouch, MusicalTouch> Touches = new Dictionary<UITouch, MusicalTouch> ();
	
		public MusicalTouch StartNote(UITouch touch, int note)
		{
			var initialVolume = volumeStrategy.TouchesDown (touch);

			var musicTouch = new MusicalTouch () {
				Note = note,
				Touch = touch,
				Volume = initialVolume
			};

			Touches [touch] = musicTouch;

			// Get the volume for the note

			// Play the midi note at that volume
			//midi.PlayNote (note, 1, initialVolume);

			if (volumeStrategy.UseInitialTouch) {
				midi.NoteOn (note, initialVolume);
			} else {
				StartWaitingForLatencyTime (musicTouch, note, volumeStrategy.MillisecondsFromFirstTouch);
			}
			musicTouch.DidChangeVisuals = true;

			return musicTouch;
		}

		async Task StartWaitingForLatencyTime(MusicalTouch touch, int note, int delayAmount)
		{
			await Task.Delay (delayAmount);

			midi.NoteOn (note, touch.CalculatedVolume);
			touch.DidChangeVisuals = true;
		}

		public MusicalTouch UpdateNote(UITouch touch, int note)
		{
			// Get the note
			var currentNote = Touches[touch];
			if (currentNote == null)
				return null;

			// Get the volume for the current touch
			var newVolume = volumeStrategy.TouchesDown (touch);

				// See if the musical note is different
			if (note != currentNote.Note) {
				// We need to change the note and the volume

				if (volumeStrategy.UseInitialTouch) {
					midi.NoteOff (currentNote.Note);

					// Start Playing the next note
					midi.NoteOn (note, newVolume);
				}
				currentNote.Note = note;

				if (volumeStrategy.UseInitialTouch)
					currentNote.Volume = newVolume;
				else
					currentNote.AddVolume (newVolume);

				currentNote.DidChangeVisuals = true;
			} else {
				if (newVolume != currentNote.Volume) {
					// Update the note and play the new volume
					if (volumeStrategy.UseInitialTouch) {
						midi.NoteOff (currentNote.Note);

						// Start Playing the next note
						midi.NoteOn (note, newVolume);
					}

					currentNote.Note = note;

					if (volumeStrategy.UseInitialTouch)
						currentNote.Volume = newVolume;
					else
						currentNote.AddVolume (newVolume);
				} 

				currentNote.DidChangeVisuals = false;
			}

			return currentNote;
		}

		public void EndNote(UITouch touch, int note)
		{
			var currentNote = Touches[touch];
			if (currentNote == null)
				return;

			if (note != currentNote.Note)
				midi.NoteOff (currentNote.Note);
		}
	}
}
