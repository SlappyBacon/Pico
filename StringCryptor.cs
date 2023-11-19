using System.Text;

namespace Pico.Cryptography
{
    /// <summary>
    /// A simple wrapper for the ByteCryptor class.
    /// </summary>
    public static class StringCryptor
    {
        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="text">Text to encrypt.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyIndex">The starting key index.</param>
        /// <returns>Encrypted byte array.</returns>
        public static byte[] Encrypt(string text, byte[] key, int keyIndex = 0)
        {
            if (text == null || key == null || key.Length == 0) return null;
            if (keyIndex < 0 || keyIndex >= key.Length) keyIndex = 0;


            var bytes = Encoding.UTF8.GetBytes(text);
            ByteCryptor.Encrypt(bytes, key, ref keyIndex);
            return bytes;
        }
        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="bytes">Bytes to decrypt.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyIndex">The starting key index.</param>
        /// <returns>Decrypted string.</returns>
        public static string Decrypt(byte[] bytes, byte[] key, int keyIndex = 0)
        {
            if (bytes == null || key == null || key.Length == 0) return null;
            if (keyIndex < 0 || keyIndex >= key.Length) keyIndex = 0;

            ByteCryptor.Decrypt(bytes, key, ref keyIndex);
            var text = Encoding.UTF8.GetString(bytes);
            return text;
        }
    }
}
