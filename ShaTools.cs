using System.Security.Cryptography;
using System.Text;
namespace Pico.Cryptography
{
    /// <summary>
    /// Quickly hash things.
    /// </summary>
    public static class ShaTools
    {
        #region Sha1
        //SHA1
        public static byte[] Sha1HashFromFile(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            using (var sha = SHA1.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha1Hash(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            using (var sha = SHA1.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha1Hash(byte[] bytes)
        {
            using (var sha = SHA1.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        #endregion
        #region Sha256
        //SHA256
        public static byte[] Sha256HashFromFile(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            using (var sha = SHA256.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha256Hash(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            using (var sha = SHA256.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha256Hash(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        #endregion
        #region Sha384
        //SHA384
        public static byte[] Sha384HashFromFile(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            using (var sha = SHA384.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha384Hash(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            using (var sha = SHA384.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha384Hash(byte[] bytes)
        {
            using (var sha = SHA384.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        #endregion
        #region Sha512
        //SHA512
        public static byte[] Sha512HashFromFile(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            using (var sha = SHA512.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha512Hash(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            using (var sha = SHA512.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        public static byte[] Sha512Hash(byte[] bytes)
        {
            using (var sha = SHA512.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }
            return bytes;
        }
        #endregion
    }
}
