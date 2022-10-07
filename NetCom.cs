using Pico.Arrays;
using Pico.Conversion;
using Pico.Cryptography;
using Pico.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Pico.Networking
{
    /// <summary>
    /// A tool which allows you to communicate
    /// with other computers over a network easily
    /// via TCP.  This class contains many 
    /// useful tools for reading/writing raw data.
    /// </summary>
    public class NetCom : IDisposable
    {
        public Stream Stream { get { return stream; } }
        public string IpAddress { get { return IpTools.ClientIp(client); } }
        TcpClient client = null;
        NetworkStream stream = null;
        #region Constructors & Disposal
        /// <summary>
        /// Listen for connection request at 'port'
        /// </summary>
        /// <param name="port"></param>
        public NetCom(int port)
        {
            //Get client
            TcpListener listener = new TcpListener(port);
            listener.Start();
            client = listener.AcceptTcpClient();
            listener.Stop();
            Setup(client);
        }

        /// <summary>
        /// Send connection request,
        /// then try to setup connection.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public NetCom(string ip, int port)
        {
            GetTcpClient(ip,port);
            Setup(client);
        }



        bool GetTcpClient(string ip, int port)
        {
            //Get client
            try
            {
                client = new TcpClient(ip, port);
                return true;
            }
            catch
            {
                client = null;
                return false;
            }
        }


        bool Setup(TcpClient client)
        {
            if (client == null) return false;
            try
            {
                stream = client.GetStream();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public void Dispose()
        {
            if (client != null) client.Dispose();
            if (stream != null) stream.Dispose();
        }
        #endregion
        #region Read / Write Text
        /// <summary>
        /// Write text to the stream.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <returns></returns>
        public bool WriteText(string text)
        {
            //Early return
            if (text == null) return false;
            if (stream == null) return false;

            //Convert text to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            //Write bytes count
            var wroteCount = StreamTools.WriteInt(stream, bytes.Length);
            if (!wroteCount) return false;

            //Write encrypted bytes
            var wroteMsg = StreamTools.WriteBytes(stream, bytes);
            if (!wroteMsg) return false;
            return true;
        }
        /// <summary>
        /// Read text from the stream.
        /// </summary>
        /// <returns></returns>
        public string ReadText(int maxLength = -1)
        {
            //Early return
            if (stream == null) return null;

            //Read count
            int count = StreamTools.ReadInt(stream);

            //Read bytes
            var bytes = StreamTools.ReadBytes(stream, count);
            if (bytes == null) return null;

            //Convert bytes to text
            var result = Encoding.UTF8.GetString(bytes);
            return result;
        }
        #endregion
        #region Read / Write Int
        /// <summary>
        /// Write an int to the stream.
        /// </summary>
        /// <param name="number">Number to write.</param>
        /// <returns></returns>
        public bool WriteInt(int number)
        {
            //Early return
            if (stream == null) return false;

            //Convert int to bytes
            byte[] bytes = BitConverter.GetBytes(number);

            //Write encrypted bytes
            StreamTools.WriteBytes(stream, bytes);
            return true;
        }
        /// <summary>
        /// Read an int from the stream.
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            //Early return
            if (stream == null) return 0;

            //Read bytes
            byte[] bytes = StreamTools.ReadBytes(stream, 4);
            if (bytes == null) return 0;

            //Convert bytes to type
            var result = BitConverter.ToInt32(bytes);
            return result;
        }
        #endregion
        #region Read / Write Long
        /// <summary>
        /// Write a long to the stream.
        /// </summary>
        /// <param name="number">Number to write.</param>
        /// <returns></returns>
        public bool WriteLong(long number)
        {
            //Early return
            if (stream == null) return false;

            //Convert type to bytes
            byte[] bytes = BitConverter.GetBytes(number);

            //Write bytes
            StreamTools.WriteBytes(stream, bytes);
            return true;
        }
        /// <summary>
        /// Read a long from the stream.
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            //Early return
            if (stream == null) return 0;

            //Read bytes
            byte[] bytes = StreamTools.ReadBytes(stream, 8);


            //Convert to type
            long result = BitConverter.ToInt64(bytes);
            return result;
        }
        #endregion
        #region Read / Write Byte[]
        /// <summary>
        /// Write bytes to the stream.
        /// </summary>
        /// <param name="bytes">Bytes to write.</param>
        /// <returns></returns>
        public bool WriteByteArray(byte[] bytes)
        {
            //Early return
            if (bytes == null) return false;
            if (stream == null) return false;

            //Write buffer size
            StreamTools.WriteInt(stream, bytes.Length);

            //Write bytes
            StreamTools.WriteBytes(stream, bytes);
            return true;
        }
        /// <summary>
        /// Read bytes from the stream.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadByteArray()
        {
            //Early return
            if (stream == null) return null;
            //Read buffer size
            int bufferSize = StreamTools.ReadInt(stream);
            //Read bytes
            var bytes = StreamTools.ReadBytes(stream, bufferSize);
            return bytes;
        }
        #endregion
        #region Read / Write Int[]
        /// <summary>
        /// Write int array to the stream.
        /// </summary>
        /// <param name="array">Ints to write.</param>
        /// <returns></returns>
        public bool WriteIntArray(int[] array)
        {

            //Early return
            if (stream == null || array == null) return false;
            if (array.Length < 1) return false;

            //Convert int[] to bytes
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

            //Write buffer size
            StreamTools.WriteInt(stream, buffer.Length);

            //Write bytes
            StreamTools.WriteBytes(stream, buffer);
            return true;
        }


        /// <summary>
        /// Read int array from stream.
        /// </summary>
        /// <returns></returns>
        public int[] ReadIntArray()
        {

            //Early return
            if (stream == null) return null;

            //Read count
            int bufferLength = StreamTools.ReadInt(stream);
            if (bufferLength < 1) return null;

            //Read bytes
            byte[] buffer = StreamTools.ReadBytes(stream, bufferLength); //4 bytes for each int
            if (buffer == null) return null;

            //Convert bytes to int[]
            int[] array = new int[bufferLength / 4];
            byte[] numBytes = new byte[4];
            for (int i = 0; i < array.Length;)
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

        public bool WriteFile(string file)
        {
            return StreamTools.WriteFile(stream, file);
        }
        public bool ReadFile(string file)
        {
            return StreamTools.ReadFile(stream, file);
        }

    }
}
