using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AudioUnit;
using Foundation;
using CoreFoundation;
using CoreMidi;
using ObjCRuntime;

namespace PianoTouch
{
	// This class is based on the work from Xamarin MVP Frank Krueger and his gist for using MIDI 
	// which can be found at: https://gist.github.com/praeclarum/143cb88ce836e476d701
	// This is a tweaked version, fitted for the use of the app and using the Unified API
	public class MidiControl 
	{
		AUGraph graph;
		AudioUnit.AudioUnit samplerUnit;

		public MidiControl ()
		{
			CreateAudioGraph ();

			LoadInstrument (1);
		}

		void CreateAudioGraph ()
		{
			graph = new AUGraph ();

			var samplerNode = graph.AddNode (AudioComponentDescription.CreateMusicDevice (AudioTypeMusicDevice.Sampler));
			var ioNode = graph.AddNode (AudioComponentDescription.CreateOutput (AudioTypeOutput.Remote));

			graph.Open ();
			graph.ConnnectNodeInput (samplerNode, 0, ioNode, 0);

			samplerUnit = graph.GetNodeInfo (samplerNode);

			graph.Initialize ();
			graph.Start ();
		}

		public void LoadInstrument (int preset)
		{
			var soundFontPath = NSBundle.MainBundle.PathForResource ("ChoriumRevA", "SF2", "SoundFonts");

			var soundFontUrl = CFUrl.FromFile (soundFontPath);

			samplerUnit.LoadInstrument (new SamplerInstrumentData (soundFontUrl, InstrumentType.SF2Preset) {
				BankLSB = SamplerInstrumentData.DefaultBankLSB,
				BankMSB = SamplerInstrumentData.DefaultMelodicBankMSB,
				PresetID = (byte)preset,
			});
		}

		public void LoadInstrument (string folder, string fontName, string extension, int preset)
		{
			var soundFontPath = NSBundle.MainBundle.PathForResource (fontName, extension, folder);

			var soundFontUrl = CFUrl.FromFile (soundFontPath);

			samplerUnit.LoadInstrument (new SamplerInstrumentData (soundFontUrl, InstrumentType.SF2Preset) {
				BankLSB = SamplerInstrumentData.DefaultBankLSB,
				BankMSB = SamplerInstrumentData.DefaultMelodicBankMSB,
				PresetID = (byte)preset,
			});
		}
			
		public async Task PlayNotes (int[] notes, int duration, double velocity = 127)
		{
			await Task.WhenAll (notes.Select (x => PlayNote (x, duration, velocity)).ToArray ());
		}

		public async Task PlayNote (int note, int duration, double velocity = 127)
		{
			var channel = 0;
			var status = (9 << 4) | channel;
			samplerUnit.MusicDeviceMIDIEvent ((byte)status, (byte)note, (byte)velocity);
			await Task.Delay (duration);
		}

		// A lot of good information on MIDI available from:
		// http://www.music-software-development.com/midi-tutorial.html

		const byte Velocity_pppp = 8;
		const byte Velocity_ppp = 20;
		const byte Velocity_pp = 31;
		const byte Velocity_p = 42;
		const byte Velocity_mp = 53;
		const byte Velocity_mf = 64;
		const byte Velocity_f = 80;
		const byte Velocity_ff = 96;
		const byte Velocity_fff = 112;
		const byte Velocity_ffff = 127;

		const byte MidiAction_NoteOn = 9; // 1001 = 9
		const byte MidiAction_NoteOff = 8; // 1000 = 8 
		const byte MidiAction_ControlTypeAllSoundOff = 0x78;
		const byte MidiAction_ChangeVolume = 12;

		public void NoteOn (int note, double velocity = Velocity_mf)
		{
			byte channel = 0;
			var status = (MidiAction_NoteOn << 4) | channel;
			samplerUnit.MusicDeviceMIDIEvent ((byte)status, (byte)note, (byte)velocity);
		}

		public void NoteOff(int note)
		{
			byte channel = 0;
			var status = (MidiAction_ControlTypeAllSoundOff << 4) | channel;
			samplerUnit.MusicDeviceMIDIEvent ((byte)status, (byte)note, (byte)0);
		}

		unsafe void HandleMidiPackets (MidiPacket[] packets)
		{
			foreach (var p in packets) {
				var bytes = (byte*)p.Bytes;

				var status = bytes [0];
				var data1 = p.Length > 1 ? bytes [1] : 0u;
				var data2 = p.Length > 2 ? bytes [2] : 0u;

				var command = status >> 4;
				if (command != 0x0F) {
					var note = (byte)(data1 & 0x7F);
					var velocity = (byte)(data2 & 0x7F); 
					samplerUnit.MusicDeviceMIDIEvent (status, note, velocity);
				} else {
					MusicDeviceSysEx (samplerUnit.Handle, p.Bytes, p.Length);
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AudioUnitStatus MusicDeviceSysEx (IntPtr inUnit, IntPtr inData, uint inLength);
	}
}

