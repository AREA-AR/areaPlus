using System;
using System.IO;

namespace TriLib {
    /// <summary>
    /// Contains IO helper functions.
    /// </summary>
	public static class FileUtils
    {
		/// <summary>
		/// Gets the file directory.
		/// </summary>
		/// <returns>The file directory.</returns>
		/// <param name="filename">Filename.</param>
        public static string GetFileDirectory(string filename) {
            var lastDash = filename.LastIndexOf('/');
            return filename.Substring(0, lastDash);
        }

		/// <summary>
		/// Gets the filename without extension.
		/// </summary>
		/// <returns>The filename without extension.</returns>
		/// <param name="filename">Filename.</param>
        public static string GetFilenameWithoutExtension(string filename) {
            var lastDash = filename.LastIndexOf('/');
            var lastDot = filename.LastIndexOf('.');
            return filename.Substring(lastDash, lastDot);
        }

		/// <summary>
		/// Synchronously loads the file data.
		/// </summary>
		/// <returns>The file data.</returns>
		/// <param name="filename">Filename.</param>
		public static byte[] LoadFileData(string filename) {
			try {
				return File.ReadAllBytes(filename);
			} catch (Exception) {
				return new byte[0];
			}
		}
    }
}

