using Pico.Arrays;
using Pico.Conversion;
using Pico.Cryptography;
using Pico.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Pico.Networking
{
    /// <summary>
    /// A tool which allows you to communicate
    /// with other computers over a network easily
    /// via TCP.  This class contains many 
    /// useful tools for reading/writing encrypted data.
    /// </summary>
    public class EncryptedNetCom : IDisposable
    {
        public string IpAddress => netCom.IpAddress;
        NetCom netCom;
        static byte[] key = new byte[256];
        #region Constructors & Disposal
        /// <summary>
        /// Listen for connection request at 'port'
        /// </summary>
        /// <param name="port"></param>
        public EncryptedNetCom(int port)
        {
            //Get client
            netCom = new NetCom(port);
            //Set first key
            SetNewKey();

        }

        /// <summary>
        /// Send connection request,
        /// then try to setup connection.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public EncryptedNetCom(string ip, int port)
        {
            //Get client
            netCom = new NetCom(ip, port);

            //Get first key
            GetNewKey();
        }
        public void Dispose()
        {
            netCom.Dispose();
        }
        #endregion
        #region Set / Get New Key
        void SetNewKey()
        {
            for (int i = 0; i < key.Length;)
            {
                key[i] = (byte)Random.Shared.Next(0, 256);  //Random byte
                i++;
            }
            netCom.WriteByteArray(key);
        }
        void GetNewKey()
        {
            var newKey = netCom.ReadByteArray();
            key = newKey;
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
            if (netCom == null) return false;

            //Convert text to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write encrypted bytes
            var wroteMsg = netCom.WriteByteArray(bytes);
            if (!wroteMsg) return false;

            //Generate new key
            SetNewKey();
            return true;
        }
        /// <summary>
        /// Read text from the stream.
        /// </summary>
        /// <returns></returns>
        public string ReadText()
        {
            //Early return
            if (netCom == null) return null;

            //Read encrypted bytes
            var bytes = netCom.ReadByteArray();
            if (bytes == null) return null;

            //Decrypt bytes
            bytes = ByteCryptor.Decrypt(bytes, key);

            //Convert bytes to text
            var result = Encoding.UTF8.GetString(bytes);

            //Get new key
            GetNewKey();
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
            if (netCom == null) return false;

            //Convert int to bytes
            byte[] bytes = BitConverter.GetBytes(number);
            
            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write encrypted bytes
            netCom.WriteByteArray(bytes);

            //Generate new key
            SetNewKey();
            return true;
        }
        /// <summary>
        /// Read an int from the stream.
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            //Early return
            if (netCom == null) return 0;

            //Read encrypted bytes
            byte[] bytes = netCom.ReadByteArray();

            //Decrypt bytes
            bytes = ByteCryptor.Decrypt(bytes, key);

            //Convert bytes to type
            var result = BitConverter.ToInt32(bytes);

            //Get new key
            GetNewKey();
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
            if (netCom == null) return false;

            //Convert type to bytes
            byte[] bytes = BitConverter.GetBytes(number);

            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write encrypted bytes
            netCom.WriteByteArray(bytes);

            //Generate new key
            SetNewKey();
            return true;
        }
        /// <summary>
        /// Read a long from the stream.
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            //Early return
            if (netCom == null) return 0;

            //Read encrypted bytes
            byte[] bytes = netCom.ReadByteArray();

            //Decrypt bytes
            bytes = ByteCryptor.Decrypt(bytes, key);

            //Convert bytes to type
            long result = BitConverter.ToInt64(bytes);

            //Get new key
            GetNewKey();
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
            if (netCom == null) return false;

            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write encrypted bytes
            netCom.WriteByteArray(bytes);

            //Generate new key
            SetNewKey();
            return true;
        }
        /// <summary>
        /// Read bytes from the stream.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadByteArray()
        {
            //Early return
            if (netCom == null) return null;
            //Read encrypted bytes
            var bytes = netCom.ReadByteArray();
            //Decrypt bytes
            bytes = ByteCryptor.Decrypt(bytes, key);
            //Get new key
            GetNewKey();
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
            if (netCom == null || array == null) return false;
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

            //Encrypt bytes
            buffer = ByteCryptor.Encrypt(buffer, key);

            //Write Encrypted Bytes
            netCom.WriteByteArray(buffer);

            //Generate new key
            SetNewKey();
            return true;
        }


        /// <summary>
        /// Read int array from stream.
        /// </summary>
        /// <returns></returns>
        public int[] ReadIntArray()
        {

            //Early return
            if (netCom == null) return null;

            //Read encrypted bytes
            byte[] buffer = netCom.ReadByteArray(); //4 bytes for each int
            if (buffer == null) return null;

            //Decrypt bytes
            buffer = ByteCryptor.Decrypt(buffer, key);

            //Convert bytes to int[]
            int[] array = new int[buffer.Length / 4];
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

            //Get new key
            GetNewKey();
            return array;
        }
        #endregion
    }
}
