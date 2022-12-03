using Pico.Streams;
using System;
using System.IO;
namespace Pico.Logger
{
    public static class Logger
    {
        static FileStream fileStream;
        static string outputPath = null;
        public static string OutputPath 
        { 
            get { return outputPath; } 
            set { SetOutputPath(value); } 
        }

        static void SetOutputPath(string newOutputPath)
        {

            //Close existing stream, if open
            //Try to open a new stream

            if (newOutputPath == outputPath) return;
            if (outputPath != null) fileStream.Dispose();
            outputPath = newOutputPath;
            if (outputPath != null) fileStream = new FileStream(outputPath, FileMode.Append, FileAccess.Write);
        }

        public static void Log(string text)
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
