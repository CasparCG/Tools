using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using CasparCG.Conformer.Core;
using CasparCG.Conformer.Core.Events;
using Settings = CasparCG.Conformer.Shell.Properties.Settings;

namespace CasparCG.Conformer.Shell
{
    public class Program
    {
        private static bool KeepRunning = true;

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleClosingHandler);
            Console.Title = string.Format("{0} {1}", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName, FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

            ThreadPool.SetMaxThreads(Environment.ProcessorCount + Settings.Default.MaxThreadsOffset, Environment.ProcessorCount + Settings.Default.MaxThreadsOffset);

            EventManager.Instance.TranscodingChanged += TranscodingChangedHandler;
            EventManager.Instance.TranscodingStarted += TranscodingStartedHandler;
            EventManager.Instance.TranscodingFinished += TranscodingFinishedHandler;

            Console.WriteLine("Waiting for work...");

            using (Transcoder transcoder = new Transcoder())
            {
                transcoder.Start(Settings.Default.LogPath, Settings.Default.InputPath, Settings.Default.OutputPath, Settings.Default.DeleteInputWhenFinished, Settings.Default.ClearOutputOnStartup, Settings.Default.LookForNewFilesOnStartup);

                while (Program.KeepRunning) { }
            }
        }

        /// <summary>
        /// Consoles the closing handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ConsoleCancelEventArgs"/> instance containing the event data.</param>
        private static void ConsoleClosingHandler(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program.KeepRunning = false;
        }

        /// <summary>
        /// Transcodings the changed handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CasparCG.Conformer.Core.Events.TranscodingChangedEventArgs"/> instance containing the event data.</param>
        private static void TranscodingChangedHandler(object sender, TranscodingChangedEventArgs e)
        {
            Console.Clear();

            lock (e.Items)
            {
                if (e.Items.Count == 0)
                    Console.WriteLine("Waiting for work...");

                foreach (var item in e.Items)
                {
                    Console.Write("{0}", item.Key);

                    if (item.Value == "Waiting")
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine(" {0}", item.Value);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        /// <summary>
        /// Transcodings the started handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        private static void TranscodingStartedHandler(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine(string.Format("[{0}] {1} {2}", DateTime.Now.ToLongTimeString(), "Start transcoding file", e.Name));
        }

        /// <summary>
        /// Transcodings the finished handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        private static void TranscodingFinishedHandler(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine(string.Format("[{0}] {1} {2}", DateTime.Now.ToLongTimeString(), "Finished transcoding file", e.Name));
        }
    }
}
