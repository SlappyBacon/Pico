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
    /// useful tools for reading/writing data.
    /// 
    /// You can also encrypt all traffic.
    /// 
    /// </summary>
    public class NetCom : IDisposable
    {
        public bool DataWaiting { get { return stream.DataAvailable; } }
        static byte[] key = new byte[256];
        public bool IsConnected
        {
            // Not currently functioning properly
            get
            {
                if (client == null) return false;
                if (!client.Connected) return false;
                return true;
            } 
        }
        public string IpAddress { get { return ip; } }
        string ip = null;
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

            //Set first key
            SetNewKey();

        }

        /// <summary>
        /// Send connection request,
        /// then try to setup connection.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public NetCom(string ip, int port)
        {
            //Get client
            try
            {
                client = new TcpClient(ip, port);
            }
            catch
            {
                client = null;
            }
            Setup(client);

            //Get first key
            GetNewKey();
        }

        bool Setup(TcpClient client)
        {
            if (client == null) return false;
            try
            {
                stream = client.GetStream();
                ip = IpTools.ClientIp(client);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public void Dispose()
        {
            ip = null;
            if (client != null) client.Dispose();
            if (stream != null) stream.Dispose();
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
            StreamTools.WriteBytes(stream, key);
        }
        void GetNewKey()
        {
            var newKey = StreamTools.ReadBytes(stream, key.Length);
            key = newKey;
        }
        #endregion
        #region Read / Write Text
        //MAKE ALL LIKE THIS :)
        public bool WriteText(string text)
        {
            //Early return
            if (text == null) return false;
            if (stream == null) return false;

            //Convert text to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write bytes count
            var wroteCount = StreamTools.WriteInt(stream, bytes.Length);
            if (!wroteCount) return false;

            //Write encrypted bytes
            var wroteMsg = StreamTools.WriteBytes(stream, bytes);
            if (!wroteMsg) return false;

            //Generate new key
            SetNewKey();
            return true;
        }
        public string ReadText()
        {
            //Early return
            if (stream == null) return null;

            //Read count
            int count = StreamTools.ReadInt(stream);

            //Read encrypted bytes
            var bytes = StreamTools.ReadBytes(stream, count);
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
        public bool WriteInt(int number)
        {
            //Early return
            if (stream == null) return false;

            //Convert int to bytes
            byte[] bytes = BitConverter.GetBytes(number);
            
            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write encrypted bytes
            StreamTools.WriteBytes(stream, bytes);

            //Generate new key
            SetNewKey();
            return true;
        }
        public int ReadInt()
        {
            //Early return
            if (stream == null) return 0;

            //Read encrypted bytes
            byte[] bytes = StreamTools.ReadBytes(stream, 4);

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
        public bool WriteLong(long number)
        {
            //Early return
            if (stream == null) return false;

            //Convert type to bytes
            byte[] bytes = BitConverter.GetBytes(number);

            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write encrypted bytes
            StreamTools.WriteBytes(stream, bytes);

            //Generate new key
            SetNewKey();
            return true;
        }
        public long ReadLong()
        {
            //Early return
            if (stream == null) return 0;

            //Read encrypted bytes
            byte[] bytes = StreamTools.ReadBytes(stream, 8);

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
        public bool WriteByteArray(byte[] bytes)
        {
            //Early return
            if (bytes == null) return false;
            if (stream == null) return false;

            //Encrypt bytes
            bytes = ByteCryptor.Encrypt(bytes, key);

            //Write buffer size
            StreamTools.WriteInt(stream, bytes.Length);

            //Write encrypted bytes
            StreamTools.WriteBytes(stream, bytes);

            //Generate new key
            SetNewKey();
            return true;
        }

        public byte[] ReadByteArray()
        {
            //Early return
            if (stream == null) return null;
            //Read buffer size
            int bufferSize = StreamTools.ReadInt(stream);
            //Read encrypted bytes
            var bytes = StreamTools.ReadBytes(stream, bufferSize);
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
        /// <param name="array">What to write</param>
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

            //Encrypt bytes
            buffer = ByteCryptor.Encrypt(buffer, key);

            //Write buffer size
            StreamTools.WriteInt(stream, buffer.Length);

            //Write Encrypted Bytes
            StreamTools.WriteBytes(stream, buffer);

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
            if (stream == null) return null;

            //Read count
            int bufferLength = StreamTools.ReadInt(stream);
            if (bufferLength < 1) return null;

            //Read encrypted bytes
            byte[] buffer = StreamTools.ReadBytes(stream, bufferLength); //4 bytes for each int
            if (buffer == null) return null;

            //Decrypt bytes
            buffer = ByteCryptor.Decrypt(buffer, key);

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

            //Get new key
            GetNewKey();
            return array;
        }
        #endregion




        //BE AWARE\\
        //Encryption not working with these...
        //Finish :)
        #region Pipe In and Out(WIP)
        public bool PipeIn(Stream fromStream, long count = 1, int bufferSize = 1)
        {
            //Read from local stream , encrypt it, then send it to partner
            //Count is also the remaining bytes

            //Early return
            if (count < 1 || bufferSize < 1) return false;

            long startCount = count;

            if (bufferSize > count) bufferSize = (int)count;//No sense wasting memory
            byte[] buffer = new byte[bufferSize];


            long failed = 0;
            long maxFailed = (int)count / bufferSize / 10;    //10% of chunks can be lost before disconnecting
            Console.WriteLine($"MAX FAILS: {maxFailed}");

            bool nextChunk = true;
            string checkHash = null;
            int len = bufferSize;   //bytes this chunk

            while (true)
            {
                if (fromStream == null || stream == null) return false;

                //When the last chunk is verified, load a new chunk
                if (nextChunk)
                {
                    //Fill buffer with next chunk
                    if (count < len) len = (int)count;

                    try
                    {
                        //Read raw bytes
                        fromStream.Read(buffer, 0, len);
                        
                    }
                    catch
                    {
                        Console.WriteLine("bad chunk");
                        failed++;
                        if (failed == maxFailed) return false;
                        continue;
                    }

                    //Encrypt bytes
                    //buffer = ByteCryptor.Encrypt(buffer, key);

                    //Calculate hash
                    checkHash = ConvertTools.ByteArrayToHex(ShaTools.Sha1Hash(buffer));

                    nextChunk = false;
                }

                //Write encrypted bytes
                try
                {
                    stream.Write(buffer, 0, len);
                }
                catch
                {
                    Console.WriteLine("bad chunk");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }

                /////CONTINUE HERE :)

                //Send hash
                var wroteHash = StreamTools.WriteText(stream, checkHash);
                if (!wroteHash)
                {
                    Console.WriteLine("bad chunk");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }

                //Read if hash is good
                var matchReply = StreamTools.ReadText(stream, 0);
                if (matchReply == null)
                {
                    Console.WriteLine("bad chunk");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }


                //Reply received
                if (matchReply != "ok")
                {
                    Console.WriteLine("bad chunk");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }

                //Okay

                nextChunk = true;
                count -= len;
                if (count < 1) break;   //End
                //Next
            }
            //Generate new key
            SetNewKey();
            return true;
        }

        public bool PipeOut(Stream toStream, long count = 1, int bufferSize = 1)
        {
            //Read stream from partner, decrypt it, then write it to local stream
            //Count is also the remaining bytes

            //Early return
            
            if (count < 1 || bufferSize < 1) return false;

            long startCount = count;

            if (bufferSize > count) bufferSize = (int)count;//No sense wasting memory
            byte[] buffer = new byte[bufferSize];

            long failed = 0;
            long maxFailed = (int)count / bufferSize / 10;    //10% packet loss
            Console.WriteLine($"MAX FAILS: {maxFailed}");

            int len = bufferSize;

            while (true)
            {
                if (stream == null || toStream == null) return false;

                if (count < len) len = (int)count;

                //DO MORE BACK-FORTH COMMUNICATION...  THE LOOP ISN'T RESETTING PROPERLY BECAUSE
                //THE CLIENT LOSES SYNC WITH SERVER.
                /*//DEBUG PACKET LOSS
                if (Random.Shared.NextDouble() < 0.5f)
                {
                    Console.WriteLine("bad chunk (TEST PACKET LOSS)");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }*/

                //Read encrypted bytes
                try
                {
                    stream.Read(buffer, 0, len);
                }
                catch
                {
                    Console.WriteLine("bad chunk (fail to read buffer)");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }

                //Check hash of bytes
                var hash = ConvertTools.ByteArrayToHex(ShaTools.Sha1Hash(buffer));

                //Read hash to expect
                var checkHash = StreamTools.ReadText(stream, 0);
                if (checkHash == null)
                {
                    Console.WriteLine("bad chunk (fail to read hash)");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }

                

                //Write if match
                if (hash != checkHash)
                {
                    Console.WriteLine("bad chunk (hash mismatch)");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }



                


                //Decrypt bytes
                //buffer = ByteCryptor.Decrypt(buffer, key);

                //Write chunk bytes
                try
                {
                    toStream.Write(buffer, 0, len);
                }
                catch
                {
                    Console.WriteLine("bad chunk (fail to write to stream)");
                    failed++;
                    if (failed == 10) return false;
                    continue;
                }

                

                var writeOk = StreamTools.WriteText(stream, "ok");
                if (!writeOk)
                {
                    Console.WriteLine("bad chunk (fail to write text)");
                    failed++;
                    if (failed == maxFailed) return false;
                    continue;
                }

                
                count -= len;
                if (count < 1) break;   //End
                //Next chunk
            }
            //Get new key
            GetNewKey();
            return true;
        }
        #endregion  //REWORK!
        #region Read / Write File (WIP)
        /// <summary>
        /// Pipe bytes from file to network stream
        /// Bytes are encrypted
        /// </summary>
        /// <param name="readFilePath">Path to read from</param>
        /// <returns></returns>
        public bool WriteFile(string readFilePath)
        {
            if (stream == null) return false;

            int bufferSize = 2048;
            //int bufferSize = 2;

            FileInfo info = new FileInfo(readFilePath);

            //Write a long to indicate file size in bytes
            var sentLength = StreamTools.WriteLong(stream, info.Length);
            if (info.Length < 1 || !sentLength) return false;

            bool piped;
            using (FileStream fileReader = new FileStream(readFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
            {
                piped = PipeIn(fileReader, info.Length, bufferSize);
            }
            return piped;
        }
        /// <summary>
        /// Pipe bytes from network stream to file
        /// Bytes are encrypted
        /// </summary>
        /// <param name="writeFilePath">Path to write to</param>
        /// <returns></returns>
        public bool ReadFile(string writeFilePath)
        {
            if (stream == null) return false;

            int bufferSize = 1000000;
            //int bufferSize = 2;

            //Read a long to indicate file size in bytes
            var length = StreamTools.ReadLong(stream);
            if (length < 1) return false;

            bool piped;
            using (FileStream fileWriter = new FileStream(writeFilePath, FileMode.Create, FileAccess.Write, FileShare.Write, bufferSize))
            {
                piped = PipeOut(fileWriter, length, bufferSize);
            }
            return piped;
        }
        #endregion  
    }
}
