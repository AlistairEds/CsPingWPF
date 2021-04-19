using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CsPingWPF.Models {
	public class IniModel {

		[DllImport ("kernel32.dll")]
		public static extern long WritePrivateProfileString ( string section, string key, string value, string filepath );

		[DllImport ("kernel32.dll")]
		private static extern int GetPrivateProfileString ( string section, string key, string def, StringBuilder returnvalue, int buffersize, string filepath );

		private static string IniFilePath;
		/// <summary>
		/// 若读取失败则返回-1
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void GetValue ( string section, string key, out string value ) {

			StringBuilder stringBuilder = new StringBuilder ();
			GetPrivateProfileString (section, key, "-1", stringBuilder, 1024, IniFilePath);
			value = stringBuilder.ToString ();

		}
		public void SetValue ( string section, string key, string value ) {
			WritePrivateProfileString (section, key, value, IniFilePath);
		}

		public void SetFilePath ( string path ) {
			IniFilePath = path;
		}
	}
}
