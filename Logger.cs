using Pico.Files;
using Pico.Streams;
using System;
using System.IO;
using Xamarin.Forms.Shapes;

namespace Pico.Logger
{
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

        public void Write(string text, bool printToConsole = true)
        {
            lock (_lock)
            {
                //Write to console
                if (printToConsole) Console.WriteLine(text);

                //Write to file?
                if (FileStream == null) return;
                if (!StreamTools.WriteText(FileStream, text)) return;
                FileStream.Flush();
            }
        }
        public void WriteLine(string text, bool printToConsole = true)
        {
            if (TimeStamp) text = $"[{UtcTimeStamp}] {text}";
            lock (_lock)
            {
                //Write to console
                if (printToConsole) Console.WriteLine(text);

                //Write to file?
                if (FileStream == null) return;
                if (!StreamTools.WriteText(FileStream, text)) return;
                if (!StreamTools.WriteText(FileStream, "\n")) return;
                FileStream.Flush();
            }
        }
        public void Dispose()
        {
            lock (_lock)
            {
                FileStream?.Dispose();
            }
        }





    }
}
