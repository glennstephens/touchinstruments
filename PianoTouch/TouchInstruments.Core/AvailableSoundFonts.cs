using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
				
		}

		public async Task LoadSoundFonts ()
		{
			
		}
	}
}