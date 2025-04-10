using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
namespace Pico.Credentials.Login
{
    struct Password
    {
        [JsonProperty]
        byte[] _hash;
        [JsonIgnore]
        byte[] Hash => _hash;

        public Password(string text) => Set(text);
        void Set(string text) => _hash = ToHash(text);

        public bool Try(string text)
        {
            var textHash = ToHash(text);
            var areSame = ArraysAreSame(textHash, Hash);
            return areSame;
        }

        public bool Change(string oldValue, string newValue)
        {
            var confirmed = Try(oldValue);
            if (!confirmed) return false;
            Set(newValue);
            return true;
        }

        byte[] ToHash(string text)
        {
            //Null check
            if (text == null) text = "";
            //Get text bytes
            var textBytes = Encoding.UTF8.GetBytes(text);
            //Encrypt?
            //Hash
            var sha = SHA512.Create();
            var hash = sha.ComputeHash(textBytes);
            sha.Dispose();
            return hash;
        }

        bool ArraysAreSame(in byte[] arr1, in byte[] arr2)
        {
            if (arr1 == null || arr2 == null) return false;

            //Both not null
            if (arr1.Length != arr2.Length) return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i]) return false;
            }

            return true;
        }
    }
}
