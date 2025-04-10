using Pico.ConsoleTools;
using Pico.Streams;
namespace Pico.Files
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
        public string FilePath
        {
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
        public void WriteLine(string text, bool timeStamp = false)
        {
            if (timeStamp) text = PrintTools.TimeStamp(text);
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
