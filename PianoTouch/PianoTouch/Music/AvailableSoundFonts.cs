using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace PianoTouch
{
	public class SoundFontEntry
	{
		string filename = null;

		public string Filename {
			get
			{
				return filename;
			} 
			set 
			{
				filename = value;
				CalculateDisplayName ();
			}
		}

		void CalculateDisplayName()
		{
			var filenameOnly = System.IO.Path.GetFileNameWithoutExtension (Filename);
			var result = filenameOnly.Replace ('_', ' ');

			this.DisplayName = result;
		}

		public string DisplayName {
			get;
			set;
		}

		public bool IsUserSoundFont {
			get;
			set;
		}
	}

	public class AvailableSoundFonts
	{		
		List<SoundFontEntry> SoundFonts = new List<SoundFontEntry>();

		//TODO: Add in file monitoring to pick up new/deleted sound fonts
		public AvailableSoundFonts ()
		{
			LoadSoundFonts ();
		}

		void AddFilesFrom(string folder, bool isUserSF)
		{
			new List<string> (Directory.GetFiles (folder))
				.ForEach (f => SoundFonts.Add (new SoundFontEntry () { Filename = f, IsUserSoundFont = isUserSF }));
		}

		public void LoadSoundFonts ()
		{
			AddFilesFrom("SoundFonts", false);
			AddFilesFrom(Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), true);
		}

		int CurrentPosition = 0;

		public SoundFontEntry GetCurrentSoundFont()
		{
			if (SoundFonts.Count == 0)
				return null;

			return SoundFonts [CurrentPosition];
		}

		public SoundFontEntry MoveNext()
		{
			if (SoundFonts.Count == 0)
				return null;

			CurrentPosition++;
			if (CurrentPosition >= SoundFonts.Count)
				CurrentPosition = 0;

			return SoundFonts [CurrentPosition];
		}

		public SoundFontEntry MoveBack()
		{
			if (SoundFonts.Count == 0)
				return null;

			CurrentPosition--;
			if (CurrentPosition < 0)
				CurrentPosition = SoundFonts.Count - 1;

			return SoundFonts [CurrentPosition];
		}
	}
}