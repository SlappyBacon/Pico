using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pico.Files;

namespace Pico.Files
{
    public static class DiskObject
    {
        public static T LoadFind<T>(string directoryPath, Func<T, bool> determinant)
        {
            if (!Directory.Exists(directoryPath)) return default(T);

            T result = default(T);

            CancellationTokenSource cts = new CancellationTokenSource();
            FileTools.ForEachFilePathParallel(directoryPath, CheckPath, false, true, cts.Token);
            cts.Dispose();

            return result;

            void CheckPath(string filePath)
            {
                T loaded = Load<T>(filePath);

                var match = determinant(loaded);

                if (match)
                {
                    result = loaded;
                    cts.Cancel();
                }
            }
        }
        public static List<T> LoadFindAll<T>(string directoryPath, Func<T, bool> determinant)
        {
            object padlock = new object();

            List<T> result = new List<T>();

            if (!Directory.Exists(directoryPath)) return result;
            
            FileTools.ForEachFilePathParallel(directoryPath, CheckPath, false, true);

            return result;

            void CheckPath(string filePath)
            {
                T loaded = Load<T>(filePath);

                var match = determinant(loaded);

                if (match)
                {
                    lock (padlock)
                    {
                        result.Add(loaded);
                    }
                }
            }
        }

        public static T Load<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default(T);
            var json = File.ReadAllText(filePath);
            return FromJson<T>(json);
        }
        public static bool Save<T>(T item, string filePath)
        {
            var json = ToJson(item);
            File.WriteAllText(filePath, json);
            return true;
        }

        static string ToJson<T>(T item)
        {
            try
            {
                return JsonConvert.SerializeObject(item, Formatting.Indented);
            }
            catch
            {
                return null;
            }
        }
        static T FromJson<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        } 
    }
}
