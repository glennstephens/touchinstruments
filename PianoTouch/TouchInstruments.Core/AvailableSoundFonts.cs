using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace TouchInstruments.Core
{
	public class SoundFontEntry
	{
		string _filename = null;

		public string Filename {
			get
			{
				return _filename;
			} 
			set 
			{
				_filename = value;
				CalculateDisplayName ();
			}
		}

		void CalculateDisplayName()
		{
			var filenameOnly = System.IO.Path.GetFileNameWithoutExtension (Filename);
			var result = "";

			foreach (char c in filenameOnly)
				if (Char.IsLetterOrDigit (c)) {
					if (Char.IsUpper (c)) {
						result += " ";
					}
					result += c;
				}

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