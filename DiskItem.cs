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
    public static class DiskItem
    {
        public static bool LoadFind<T>(string directoryPath, Func<T, bool> determinant, out T item)
        {
            if (!Directory.Exists(directoryPath))
            {
                item = default;
                return false;
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            
            bool found = false;
            T foundItem = default;
            FileTools.ForEachFilePathParallel(directoryPath, CheckPath, false, true, cts.Token);
            if (found) item = foundItem;
            else item = default;
            
            cts.Dispose();
            
            return found;

            void CheckPath(string filePath)
            {
                T loadedItem;
                bool loaded = Load(filePath, out loadedItem);
                if (!loaded) return;

                var match = determinant(loadedItem);

                if (match)
                {
                    foundItem = loadedItem;
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
                T loadedItem;
                bool loaded = Load(filePath, out loadedItem);
                if (!loaded) return;

                var match = determinant(loadedItem);

                if (match)
                {
                    lock (padlock)
                    {
                        result.Add(loadedItem);
                    }
                }
            }
        }

        public static bool Load<T>(string filePath, out T item)
        {
            if (!File.Exists(filePath))
            {
                item = default;
                return false;
            }
            var json = File.ReadAllText(filePath);
            var converted = FromJson(json, out item);
            return converted;
        }
        public static bool Save<T>(string filePath, in T item)
        {
            string json;
            var converted = ToJson(item, out json);
            if (!converted) return false;
            File.WriteAllText(filePath, json);
            return true;
        }

        static bool ToJson<T>(T item, out string json)
        {
            try
            {
                json = JsonConvert.SerializeObject(item, Formatting.Indented);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        }
        static bool FromJson<T>(string json, out T item)
        {
            try
            {
                item = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch
            {
                item = default(T);
                return false;
            }
        } 
    }
}
