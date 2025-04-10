using Pico.Cryptography;
using Pico.Files;
using System.Text;
namespace Pico.Streams
{
    /// <summary>
    /// StreamTools is for reading and writing RAW DATA to and from streams.
    /// </summary>
    public static class StreamTools
    {
        static int bufferSize = 1048576 * 8; //1mb * megabytes

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
        #region Write / Read File
        public static bool WriteFile(Stream stream, string file)
        {
            FileTools.FormatPath(ref file);
            if (!File.Exists(file)) return false;

            long fileSize = new FileInfo(file).Length;

            //Send file size
            bool wroteFileSize = StreamTools.WriteLong(stream, fileSize);

            using (FileStream readStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[bufferSize];

                try
                {
                    //Write all file bytes to stream, in chunks
                    while (true)
                    {
                        //Read bytes chunk from file
                        int readByteCount = readStream.Read(buffer, 0, buffer.Length);
                        if (readByteCount == 0) break;

                        //Write bytes chunk to stream
                        stream.Write(buffer, 0, readByteCount);

                        Thread.Sleep(10);
                    }
                }
                catch { return false; }
            }
            return true;
        }

        //Add long? override for max file size
        public static bool ReadFile(Stream stream, string file)
        {
            //Get file size from sender
            long remainingBytes = StreamTools.ReadLong(stream);

            using (FileStream writeStream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[bufferSize];

                try
                {
                    int readByteCount;
                    while (true)
                    {
                        //Read bytes chunk from stream
                        if (remainingBytes < buffer.Length) readByteCount = stream.Read(buffer, 0, (int)remainingBytes);
                        else readByteCount = stream.Read(buffer, 0, buffer.Length);

                        if (readByteCount == 0) break;

                        remainingBytes -= readByteCount;

                        //Write bytes chunk to file
                        writeStream.Write(buffer, 0, readByteCount);

                        //Check if end of file
                        if (remainingBytes < 1) break;

                        Thread.Sleep(10);
                    }
                }
                catch { return false; }
            }
            return true;
        }
        #endregion
        #region ENCRYPTED Write / Read File IMPLEMENT WITH BYTECRYPTOR
        public static bool EncryptedWriteFile(Stream stream, string file, byte[] key)
        {
            FileTools.FormatPath(ref file);
            if (!File.Exists(file)) return false;

            long fileSize = new FileInfo(file).Length;

            int keyIndex = 0;

            //Send file size
            bool wroteFileSize = StreamTools.WriteLong(stream, fileSize);

            using (FileStream readStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[bufferSize];

                try
                {
                    //Write all file bytes to stream, in chunks
                    while (true)
                    {
                        //Read bytes chunk from file
                        int readByteCount = readStream.Read(buffer, 0, buffer.Length);
                        if (readByteCount == 0) break;

                        //Encrypt bytes chunk
                        ByteCryptor.Encrypt(buffer, key, ref keyIndex);

                        //Write bytes chunk to stream
                        stream.Write(buffer, 0, readByteCount);

                        Thread.Sleep(10);
                    }
                }
                catch { return false; }
            }
            return true;
        }

        //Add long? override for max file size
        public static bool EncryptedReadFile(Stream stream, string file, byte[] key)
        {
            //Get file size from sender
            long remainingBytes = StreamTools.ReadLong(stream);

            int keyIndex = 0;

            using (FileStream writeStream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[bufferSize];

                try
                {
                    int readByteCount;
                    while (true)
                    {
                        //Read bytes chunk from stream
                        if (remainingBytes < buffer.Length) readByteCount = stream.Read(buffer, 0, (int)remainingBytes);
                        else readByteCount = stream.Read(buffer, 0, buffer.Length);

                        if (readByteCount == 0) break;
                        remainingBytes -= readByteCount;

                        //Decrypt bytes chunk
                        ByteCryptor.Decrypt(buffer, key, ref keyIndex);

                        //Write bytes chunk to file
                        writeStream.Write(buffer, 0, readByteCount);

                        //Check if end of file
                        if (remainingBytes < 1) break;

                        Thread.Sleep(10);
                    }
                }
                catch { return false; }
            }
            return true;
        }
        #endregion
    }
}
