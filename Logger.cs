using Pico.Streams;
using System;
using System.IO;
namespace Pico.Logger
{
    public static class Logger
    {
        static object _logLock = new object();
        static FileStream fileStream;
        static string _outputPath = null;
        public static string OutputPath 
        { 
            get { return _outputPath; } 
            set { SetOutputPath(value); } 
        }

        static void SetOutputPath(string newOutputPath)
        {
            lock (_logLock)
            {
                //Close existing stream, if open
                //Try to open a new stream

                if (newOutputPath == OutputPath) return;
                fileStream?.Dispose();
                _outputPath = newOutputPath;
                if (OutputPath != null)
                {
                    fileStream = new FileStream(OutputPath, FileMode.Append, FileAccess.Write);
                }
            }
        }

        public static void Log(string text)
        {
            lock (_logLock)
            {
                //Write to console
                Console.Write(text);
                Console.Write('\n');

                //Write to file?
                if (fileStream == null) return;
                if (!StreamTools.WriteText(fileStream, text)) return;
                if (!StreamTools.WriteText(fileStream, "\n")) return;
                fileStream.Flush();
            }
        }
    }
}
