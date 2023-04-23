using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Cryptography
{
    /// <summary>
    /// Simple ecryption, but it's something.
    /// </summary>
    public static class ByteCryptor
    {
        /// <summary>
        /// Encrypt bytes.
        /// </summary>
        /// <param name="bytes">Bytes to encrypt.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyIndex">The starting key index.</param>
        /// <returns></returns>
        public static void Encrypt(byte[] bytes, byte[] key, ref int keyIndex)
        {
            if (bytes == null || bytes.Length == 0) return;
            if (key == null || key.Length == 0) return;
            if (keyIndex < 0 || keyIndex >= key.Length) keyIndex = 0;

            for (int i = 0; i < bytes.Length;)
            {
                //Shift byte
                int newByte = bytes[i] + key[keyIndex];
                while (newByte > byte.MaxValue) newByte -= 256;
                bytes[i] = (byte)newByte;

                //cycle indeces
                keyIndex++;
                if (keyIndex == key.Length) keyIndex = 0;
                i++;
            }
        }
        /// <summary>
        /// Encrypt bytes.
        /// </summary>
        /// <param name="bytes">Bytes to encrypt.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static void Encrypt(byte[] bytes, byte[] key)
        {
            int keyIndex = 0;
            Encrypt(bytes, key, ref keyIndex);
        }
        /// <summary>
        /// Encrypt file bytes.
        /// </summary>
        /// <param name="inPath">Decrypted (input) file path.</param>
        /// <param name="outPath">Encrypted (output) file path.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyIndex">The starting key index.</param>
        /// <returns></returns>
        public static async Task EncryptFile(string inPath, string outPath, byte[] key, int keyIndex = 0)
        {
            byte[] buffer = new byte[1024];
            FileStream reader = new FileStream(inPath, FileMode.Open, FileAccess.Read);
            FileStream writer = new FileStream(outPath, FileMode.CreateNew, FileAccess.Write);

            while (true)
            {
                var readCount = reader.Read(buffer, 0, buffer.Length);
                if (readCount < 1) break;

                ByteCryptor.Encrypt(buffer, key, ref keyIndex);

                writer.Write(buffer, 0, readCount);
            }

            reader.Dispose();
            writer.Dispose();
        }
        /// <summary>
        /// Decrypt bytes.
        /// </summary>
        /// <param name="bytes">Bytes to decrypt.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyIndex">The starting key index.</param>
        /// <returns></returns>
        public static void Decrypt(byte[] bytes, byte[] key, ref int keyIndex)
        {
            if (bytes == null ||  bytes.Length == 0) return;
            if (key == null || key.Length == 0) return;
            if (keyIndex < 0 || keyIndex >= key.Length) keyIndex = 0;

            for (int i = 0; i < bytes.Length;)
            {
                //Shift byte
                int newByte = bytes[i] - key[keyIndex];
                while (newByte < byte.MinValue) newByte += 256;
                bytes[i] = (byte)newByte;

                //cycle indeces
                keyIndex++;
                if (keyIndex == key.Length) keyIndex = 0;
                i++;
            }
        }
        /// <summary>
        /// Decrypt bytes.
        /// </summary>
        /// <param name="bytes">Bytes to decrypt.</param>
        /// <param name="key">The key.</param>
        public static void Decrypt(byte[] bytes, byte[] key)
        {
            int keyIndex = 0;
            Decrypt(bytes, key, ref keyIndex);
        }
        /// <summary>
        /// Decrypt file bytes.
        /// </summary>
        /// <param name="inPath">Encrypted (input) file path.</param>
        /// <param name="outPath">Decrypted (output) file path.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyIndex">The starting key index.</param>
        /// <returns></returns>
        public static async Task DecryptFile(string inPath, string outPath, byte[] key, int keyIndex = 0)
        {
            byte[] buffer = new byte[1024];
            FileStream reader = new FileStream(inPath, FileMode.Open, FileAccess.Read);
            FileStream writer = new FileStream(outPath, FileMode.CreateNew, FileAccess.Write);

            while (true)
            {
                var readCount = reader.Read(buffer, 0, buffer.Length);
                if (readCount < 1) break;

                ByteCryptor.Decrypt(buffer, key, ref keyIndex);

                writer.Write(buffer, 0, readCount);
            }

            reader.Dispose();
            writer.Dispose();
        }
    }
}