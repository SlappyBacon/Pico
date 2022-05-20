using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Cryptography;

public static class ByteCryptor
{
    /// <summary>
    /// Simple ecryption, but it's something.
    /// </summary>
    /// <param name="bytes">Bytes to encrypt.</param>
    /// <param name="key">The key.</param>
    /// <param name="keyIndex">The starting key index.</param>
    /// <returns></returns>
    public static byte[] Encrypt(byte[] bytes, byte[] key, int keyIndex = 0)
    {
        if (keyIndex < 0 || keyIndex >= key.Length) keyIndex = 0;
        if (bytes == null || key == null || bytes.Length == 0 || key.Length == 0)
        {
            return null;
        }

        
        for (int i = 0; i < bytes.Length;)
        {
            //Shift byte
            int newByte = bytes[i] + key[keyIndex];
            while (newByte > byte.MaxValue) newByte -= byte.MaxValue;
            bytes[i] = (byte)newByte;

            //cycle indeces
            keyIndex++;
            if (keyIndex == key.Length) keyIndex = 0;
            i++;
        }
        return bytes;
    }
    /// <summary>
    /// Simple ecryption, but it's something.
    /// </summary>
    /// <param name="bytes">Bytes to encrypt.</param>
    /// <param name="key">The key.</param>
    /// <param name="keyIndex">The starting key index.</param>
    /// <returns></returns>
    public static byte[] Decrypt(byte[] bytes, byte[] key, int keyIndex = 0)
    {
        if (bytes == null || key == null || bytes.Length == 0 || key.Length == 0)
        {
            return null;
        }
        if (keyIndex < 0 || keyIndex >= key.Length) keyIndex = 0;

        for (int i = 0; i < bytes.Length;)
        {
            //Shift byte
            int newByte = bytes[i] - key[keyIndex];
            while (newByte < byte.MinValue) newByte += byte.MaxValue;
            bytes[i] = (byte)newByte;

            //cycle indeces
            keyIndex++;
            if (keyIndex == key.Length) keyIndex = 0;
            i++;
        }
        return bytes;
    }
}
