using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CasparCG.Conformer.Core
{
    public class Validation
    {
        /// <summary>
        /// Checks the invalid filename.
        /// </summary>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static FileSystemEventArgs CheckInvalidFilename(FileSystemEventArgs e)
        {
            // Check for spaces in filename.
            if (!e.Name.Contains(" "))
                return e;

            string name = e.Name;
            string rename = e.Name.Replace(" ", "_");

            File.Move(string.Format("{0}/{1}", Path.GetDirectoryName(e.FullPath), name), string.Format("{0}/{1}", Path.GetDirectoryName(e.FullPath), rename));

            return new FileSystemEventArgs(e.ChangeType, Path.GetDirectoryName(e.FullPath), rename);
        }
    }
}
