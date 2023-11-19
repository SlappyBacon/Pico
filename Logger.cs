using Pico.Files;
using Pico.Streams;
using System;
using System.IO;
namespace Pico.Logger
{
    public class Logger : IDisposable
    {
        static object _logLock = new object();
        static FileStream fileStream = null;

        public Logger() { }
        public Logger(string path)
        {
            SetOutputPath(path);
        }

        public void SetOutputPath(string path)
        {
            lock (_logLock)
            {
                //Close existing stream, if open
                //Try to open a new stream

                fileStream?.Dispose();

                if (path != null)
                {
                    FileTools.FormatPath(ref path);
                    fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
                }
            }
        }
        public void Write(string text, bool printToConsole = true)
        {
            lock (_logLock)
            {
                //Write to console
                if (printToConsole) Console.WriteLine(text);

                //Write to file?
                if (fileStream == null) return;
                if (!StreamTools.WriteText(fileStream, text)) return;
                fileStream.Flush();
            }
        }
        public void WriteLine(string text, bool printToConsole = true)
        {
            lock (_logLock)
            {
                //Write to console
                if (printToConsole) Console.WriteLine(text);

                //Write to file?
                if (fileStream == null) return;
                if (!StreamTools.WriteText(fileStream, text)) return;
                if (!StreamTools.WriteText(fileStream, "\n")) return;
                fileStream.Flush();
            }
        }
        public void Dispose()
        {
            lock (_logLock)
            {
                fileStream?.Dispose();
            }
        }
    }
}
