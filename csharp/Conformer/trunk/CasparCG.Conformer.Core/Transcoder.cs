using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using CasparCG.Conformer.Core.Events;

namespace CasparCG.Conformer.Core
{
    public class Transcoder : IDisposable
    {
        private string LogPath { get; set; }
        private string InputPath { get; set; }
        private string OutputPath { get; set; }

        private bool ClearOutputOnStartup { get; set; }
        private bool DeleteInputWhenFinished { get; set; }
        private bool LookForNewFilesOnStartup { get; set; }

        private FileSystemWatcher InputWatcher { get; set; }

        private Dictionary<string, string> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transcoder"/> class.
        /// </summary>
        public Transcoder()
        {
            this.InputWatcher = new FileSystemWatcher();
            this.Items = new Dictionary<string, string>();
        }

        /// <summary>
        /// Starts the specified log path.
        /// </summary>
        /// <param name="logPath">The log path.</param>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="deleteInputWhenFinished">if set to <c>true</c> [delete input when finished].</param>
        /// <param name="clearOutputOnStartup">if set to <c>true</c> [clear output on startup].</param>
        public void Start(string logPath, string inputPath, string outputPath, bool deleteInputWhenFinished, bool clearOutputOnStartup, bool lookForNewFilesOnStartup)
        {
            this.LogPath = logPath;
            this.InputPath = inputPath;
            this.OutputPath = outputPath;
            this.DeleteInputWhenFinished = deleteInputWhenFinished;
            this.ClearOutputOnStartup = clearOutputOnStartup;
            this.LookForNewFilesOnStartup = lookForNewFilesOnStartup;

            if (!Directory.Exists(this.LogPath))
                Directory.CreateDirectory(this.LogPath);

            if (!Directory.Exists(this.InputPath))
                Directory.CreateDirectory(this.InputPath);

            if (!Directory.Exists(this.OutputPath))
                Directory.CreateDirectory(this.OutputPath);

            this.InputWatcher.Created += new FileSystemEventHandler(InputWatcherHandler);
            this.InputWatcher.Path = this.InputPath;
            this.InputWatcher.EnableRaisingEvents = true;

            if (this.ClearOutputOnStartup)
                ClearOutputLocation();

            if (this.LookForNewFilesOnStartup)
                CheckInputLocation();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.InputWatcher.Dispose();
        }

        private void ClearOutputLocation()
        {
            if (Directory.GetFiles(this.OutputPath).Length > 0)
            {
                foreach (string file in Directory.GetFiles(this.OutputPath))
                    File.Delete(file);
            }
        }

        /// <summary>
        /// Checks the input location.
        /// </summary>
        private void CheckInputLocation()
        {
            if (Directory.GetFiles(this.InputPath).Length > 0)
            {
                foreach (string file in Directory.GetFiles(this.InputPath))
                {
                    // Find the target extension in the specification.
                    if (!Specification.FindTargetExtension(Path.GetExtension(file)))
                        continue;

                    lock (this.Items)
                    {
                        this.Items.Add(Path.GetFileName(file), "Waiting");
                        EventManager.Instance.FireTranscodingChangedEvent(this, new TranscodingChangedEventArgs() { Items = this.Items });
                    }

                    ThreadPool.QueueUserWorkItem(ProcessInput, new FileSystemEventArgs(WatcherChangeTypes.Created, this.InputPath, Path.GetFileName(file)));
                }
            }
        }

        /// <summary>
        /// Inputs the watcher handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        private void InputWatcherHandler(object sender, FileSystemEventArgs e)
        {
            // Find the target extension in the specification.
            if (!Specification.FindTargetExtension(Path.GetExtension(e.Name)))
                return;

            e = Validation.CheckInvalidFilename(e);

            lock (this.Items)
            {
                this.Items.Add(e.Name, "Waiting");
                EventManager.Instance.FireTranscodingChangedEvent(this, new TranscodingChangedEventArgs() { Items = this.Items });
            }

            ThreadPool.QueueUserWorkItem(ProcessInput, e);
        }

        /// <summary>
        /// Processes the input.
        /// </summary>
        /// <param name="param">The param.</param>
        private void ProcessInput(object param)
        {
            FileSystemEventArgs e = param as FileSystemEventArgs;

            FileStream stream = null;
            while (true)
            {
                try
                {
                    // We try to read from the file to see if it's locked by windows copying process.
                    stream = File.Open(string.Format("{0}/{1}", Path.GetDirectoryName(e.FullPath), e.Name), FileMode.Open);
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
            stream.Close();

            EventManager.Instance.FireTranscodingStartedEvent(this, e);

            lock (this.Items)
            {
                this.Items[e.Name] = "Running";
                EventManager.Instance.FireTranscodingChangedEvent(this, new TranscodingChangedEventArgs() { Items = this.Items });
            }

            using (Process process = new Process())
            {
                process.StartInfo.FileName = @"ffmpeg.exe";
                process.StartInfo.Arguments = string.Format(@"-i {0}/{1} {2} -y {3}/{4}", Path.GetDirectoryName(e.FullPath), e.Name, Specification.GetTargetCommand(Path.GetExtension(e.Name)), this.OutputPath, e.Name);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                using (StreamReader reader = process.StandardError)
                {
                    // This call will block in ReadToEnd().
                    File.WriteAllText(string.Format("{0}/{1}.log", this.LogPath, e.Name), reader.ReadToEnd());
                }

                process.WaitForExit();

                EventManager.Instance.FireTranscodingFinishedEvent(this, e);

                lock (this.Items)
                {
                    this.Items.Remove(e.Name);
                    EventManager.Instance.FireTranscodingChangedEvent(this, new TranscodingChangedEventArgs() { Items = this.Items });
                }
            }

            if (this.DeleteInputWhenFinished)
                File.Delete(string.Format("{0}/{1}", Path.GetDirectoryName(e.FullPath), e.Name));
        }
    }
}
