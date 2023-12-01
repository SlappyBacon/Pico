using Pico.Files;
using Pico.Print;
using Pico.Streams;
using System;
using System.IO;
using Xamarin.Forms.Shapes;

namespace Pico.Logger
{
    /// <summary>
    /// A tool for logging text.
    /// </summary>
    public class Logger : IDisposable
    {
        object _lock = new object();
        FileStream _fileStream = null;
        bool _timestamp = false;
        string _filePath = null;

        FileStream FileStream { get { return _fileStream; } }
        public string FilePath {
            get { return _filePath; }
            set
            {
                lock (_lock)
                {
                    FileTools.FormatPath(ref value);
                    _filePath = value;

                    //Close existing stream, if open
                    FileStream?.Dispose();

                    //Try to open a new stream
                    if (FilePath == null) return;
                    _fileStream = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
                }
            }
        }
        public bool TimeStamp 
        { 
            get { return _timestamp; } 
            set { _timestamp = value; }
        }
        string UtcTimeStamp { get { return DateTime.UtcNow.ToString(); } }
        
        public Logger() { }

        /// <summary>
        /// Writes text to log.
        /// </summary>
        /// <param name="text">Text.</param>
        public void Write(string text)
        {
            lock (_lock)
            {
                //Write to console
                Console.WriteLine(text);

                //Write to file?
                if (FileStream == null) return;
                if (!StreamTools.WriteText(FileStream, text)) return;
                FileStream.Flush();
            }
        }
        /// <summary>
        /// Writes text to log.
        /// </summary>
        /// <param name="text">Text.</param>
        public void WriteLine(string text)
        {
            if (TimeStamp) text = PrintTools.TimeStamp(text);
            lock (_lock)
            {
                //Write to console
                Console.WriteLine(text);

                //Write to file?
                if (FileStream == null) return;
                if (!StreamTools.WriteText(FileStream, text)) return;
                if (!StreamTools.WriteText(FileStream, "\n")) return;
                FileStream.Flush();
            }
        }
        /// <summary>
        /// Frees memory.
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                FileStream?.Dispose();
            }
        }





    }
}
