using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Streams
{
    /// <summary>
    /// StreamTools is for reading and writing RAW DATA to and from streams.
    /// </summary>
    public static class StreamTools
    {
        #region Read / Write Text
        /// <summary>
        /// Write text to the stream.
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="text">What to write</param>
        /// <returns></returns>
        public static bool WriteText(Stream stream, string text)
        {
            if (text == null) return false;
            if (stream == null) return false;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            return WriteBytes(stream, textBytes);
        }
        /// <summary>
        /// Read text from stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="length">Length of string</param>
        /// <returns></returns>
        public static string ReadText(Stream stream, int length)
        {
            if (stream == null) return null;
            var textBytes = ReadBytes(stream, length * 4);
            if (textBytes == null) return null;
            try
            {
                return Encoding.UTF8.GetString(textBytes);
            }
            catch
            {
                return null;
            }

        }
        #endregion
        #region Read / Write Int
        /// <summary>
        /// Write int to the stream.
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="number">What to write</param>
        /// <returns></returns>
        public static bool WriteInt(Stream stream, int number)
        {
            if (stream == null) return false;

            //write bytes
            try
            {
                stream.Write(BitConverter.GetBytes(number), 0, 4);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Read int from stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns></returns>
        public static int ReadInt(Stream stream)
        {
            if (stream == null) return 0;

            //create buffer
            byte[] numBytes = new byte[4];

            //read bytes
            try
            {
                stream.Read(numBytes, 0, 4);
                return BitConverter.ToInt32(numBytes);
            }
            catch
            {
                return 0;
            }
        }
        #endregion
        #region Read / Write Int[]
        /// <summary>
        /// Write int array to the stream.
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="array">What to write</param>
        /// <returns></returns>
        public static bool WriteIntArray(Stream stream, int[] array)
        {
            if (stream == null || array == null) return false;
            if (array.Length < 1) return false;

            byte[] buffer = new byte[array.Length * 4]; //4 bytes for each int
            for (int i = 0; i < array.Length;)
            {
                //Convert to bytes
                var numBytes = BitConverter.GetBytes(array[i]);
                //Buffer bytes for array[i]
                for (int j = 0; j < 4;)
                {
                    int b = (i * 4) + j;
                    buffer[b] = numBytes[j];
                    j++;
                }
                i++;
            }
            WriteBytes(stream, buffer);
            return true;
        }
        /// <summary>
        /// Read int array from stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns></returns>
        public static int[] ReadIntArray(Stream stream, int length)
        {
            if (stream == null) return null;
            if (length < 1) return null;
            byte[] buffer = ReadBytes(stream, length * 4); //4 bytes for each int
            if (buffer == null) return null;
            int[] array = new int[length];

            byte[] numBytes = new byte[4];
            for (int i = 0; i < length;)
            {
                //Get the bytes for array[i]
                for (int j = 0; j < 4;)
                {
                    int b = (i * 4) + j;
                    numBytes[j] = buffer[b];
                    j++;
                }
                //Convert to int
                array[i] = BitConverter.ToInt32(numBytes);
                i++;
            }

            return array;
        }
        #endregion
        #region Read / Write Long
        /// <summary>
        /// Write long to the stream.
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="number">What to write</param>
        /// <returns></returns>
        public static bool WriteLong(Stream stream, long number)
        {
            if (stream == null) return false;

            //write bytes
            try
            {
                stream.Write(BitConverter.GetBytes(number), 0, 8);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Read long from stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns></returns>
        public static long ReadLong(Stream stream)
        {
            if (stream == null) return 0;

            //create buffer
            byte[] numBytes = new byte[8];

            //read bytes
            try
            {
                stream.Read(numBytes, 0, 8);
                return BitConverter.ToInt64(numBytes);
            }
            catch
            {
                return 0;
            }
        }
        #endregion
        #region Read / Write Bytes

        /// <summary>
        /// Writes raw bytes to the stream.
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="bytes">What to write</param>
        /// <returns></returns>
        public static bool WriteBytes(Stream stream, byte[] bytes)
        {
            if (bytes == null) return false;
            if (stream == null) return false;

            //write bytes
            try
            {
                stream.Write(bytes, 0, bytes.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads raw bytes from stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns></returns>
        public static byte[] ReadBytes(Stream stream, int count)
        {
            if (stream == null) return null;

            //create buffer
            byte[] readBytes = new byte[count];

            //read bytes
            try
            {
                stream.Read(readBytes, 0, count);
                return readBytes;
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Read / Write File
        /// <summary>
        /// Pipe bytes from file to a stream
        /// </summary>
        /// <param name="stream">Pipe into this stream</param>
        /// <param name="readFilePath">Path to read from</param>
        /// <param name="bufferSize">Buffer size of each 'chunk'.  Smaller chunks take longer, but also use less memory.</param>
        /// <returns></returns>
        public static bool WriteFile(Stream stream, string readFilePath, int bufferSize = 1000)
        {
            if (stream == null) return false;

            FileInfo info = new FileInfo(readFilePath);
            //Write a long to indicate file size in bytes
            var sentLength = WriteLong(stream, info.Length);
            Console.WriteLine(info.Length);
            if (info.Length < 1 || !sentLength) return false;

            bool piped;
            using (FileStream fileReader = new FileStream(readFilePath, FileMode.Open, FileAccess.Read))
            {
                Console.WriteLine("starting pipe");
                piped = Pipe(fileReader, stream, info.Length, bufferSize);
            }
            Console.WriteLine($"piped: {piped}");
            return piped;
        }
        /// <summary>
        /// Pipe bytes from stream to file
        /// </summary>
        /// <param name="stream">Pipe from this stream</param>
        /// <param name="writeFilePath">Path to write to</param>
        /// <param name="bufferSize">Buffer size of each 'chunk'.  Smaller chunks take longer, but also use less memory.</param>
        /// <returns></returns>
        public static bool ReadFile(Stream stream, string writeFilePath, int bufferSize = 1000)
        {
            if (stream == null) return false;

            //Read a long to indicate file size in bytes
            var length = ReadLong(stream);
            Console.WriteLine(length);
            if (length < 1) return false;

            bool piped;
            using (FileStream fileWriter = new FileStream(writeFilePath, FileMode.Create, FileAccess.Write))
            {
                piped = Pipe(stream, fileWriter, length, bufferSize);
            }
            return piped;
        }
        #endregion
        #region Pipe
        /// <summary>
        /// Pipe bytes from one stream to another.
        /// </summary>
        /// <param name="fromStream">Read bytes from this stream</param>
        /// <param name="toStream">Write bytes to this stream</param>
        /// <param name="count">Number of bytes to pipe</param>
        /// <param name="bufferSize">Buffer size of each 'chunk'.  Smaller chunks take longer, but also use less memory.</param>
        /// <returns></returns>
        public static bool Pipe(Stream fromStream, Stream toStream, long count = 1, int bufferSize = 1)
        {
            //Count is also the remaining bytes
            
            if (fromStream == null || toStream == null) return false;
            if (count < 1 || bufferSize < 1) return false;

            if (bufferSize > count) bufferSize = (int)count;//No sense wasting memory

            try
            {
                byte[] buffer = new byte[bufferSize];
                while (true)
                {
                    Console.WriteLine("tick");
                    if (count < buffer.Length)
                    {
                        Console.WriteLine("less");
                        fromStream.Read(buffer, 0, (int)count);
                        toStream.Write(buffer, 0, (int)count);
                        count -= (int)count;  //the end
                    }
                    else
                    {
                        Console.WriteLine("more");
                        fromStream.Read(buffer, 0, buffer.Length);
                        toStream.Write(buffer, 0, buffer.Length);
                        count -= buffer.Length;
                    }
                    
                    if (count == 0) break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
