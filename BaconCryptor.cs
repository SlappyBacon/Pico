using System.Text;

namespace Pico.Cryptography
{
    public static class StringCryptor
    {
        public static string Encrypt(string text, string key)
        {
            if (text == null || text.Length == 0) return "";
            if (key == null || key.Length == 0) return "";
            char[] textChars = text.ToCharArray();
            int keyIndex = 0;
            for (int i = 0; i < textChars.Length;)
            {
                textChars[i] += key[keyIndex];
                if (keyIndex == key.Length - 1) keyIndex = 0;
                else keyIndex++;
                i++;
            }
            StringBuilder result = new StringBuilder(textChars.Length);
            for (int i = 0; i < textChars.Length;)
            {
                result.Append(textChars[i]);
                i++;
            }
            return result.ToString();
        }
        public static string Decrypt(string text, string key)
        {
            if (text == null || text.Length == 0) return "";
            if (key == null || key.Length == 0) return "";
            char[] textChars = text.ToCharArray();
            int keyIndex = 0;
            for (int i = 0; i < textChars.Length;)
            {
                textChars[i] -= key[keyIndex];
                if (keyIndex == key.Length - 1) keyIndex = 0;
                else keyIndex++;
                i++;
            }
            StringBuilder result = new StringBuilder(textChars.Length);
            for (int i = 0; i < textChars.Length;)
            {
                result.Append(textChars[i]);
                i++;
            }
            return result.ToString();
        }
    }
}
