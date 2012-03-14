using System;
using System.IO;

namespace Bespoke.Common
{
    /// <summary>
    ///
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static string ApplicationName
        {
            get
            {
                return sApplicationName;
            }
            set
            {
                sApplicationName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string FileName
        {
            get
            {
                return sFileName;
            }
            set
            {
                sFileName = value;
            }
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        public static void Initialize()
        {
            sFileName = DEFAULT_FILENAME;
            sApplicationName = DEFAULT_APPLICATION_NAME;

            try
            {
                FileStream file = new FileStream(sFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

                // Check if file is too large (more than 2 MB), if so, recreate it.
                if (file.Length > 2 * 1024 * 1024)
                {
                    file.Close();
                    file = new FileStream(sFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite );
                }

                sWriter = (file.Length == 0 ? new StreamWriter(file, System.Text.Encoding.UTF8) : new StreamWriter(file));
                sWriter.BaseStream.Seek(0, SeekOrigin.End);
                sWriter.AutoFlush = true;

                sWriter.WriteLine(Environment.NewLine + "/// Session started at: " + DateTime.Now.ToString());
                sWriter.WriteLine("/// Application: " + sApplicationName + Environment.NewLine);
            }
            catch (IOException)
            {
                // Ignore any file exceptions
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore any file exceptions
            }
        }

        /// <summary>
        /// Writes a LogType and info/error message string to the Log file
        /// </summary>
        public static void Write(string message)
        {
            if (sWriter != null)
            {
                try
                {
                    DateTime currentTime = DateTime.Now;
                    string s = "[" + currentTime.Hour.ToString("00") + ":" +
                        currentTime.Minute.ToString("00") + ":" +
                        currentTime.Second.ToString("00") + "] " +
                        message;
                    sWriter.WriteLine(s);

                    #if DEBUG                    
                    System.Console.WriteLine(s);
                    #endif
                }
                catch (IOException)
                {
                    // Ignore any file exceptions
                }
                catch (UnauthorizedAccessException)
                {
                    // Ignore any file exceptions
                }
            }            
        }

        private static readonly string DEFAULT_FILENAME = "Log.txt";
        private static readonly string DEFAULT_APPLICATION_NAME = "Unspecified Application";

        private static string sFileName;
        private static string sApplicationName;
        private static StreamWriter sWriter;
    }
}
