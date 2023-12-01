using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace Pico.Files
{
    /// <summary>
    /// A collection of tools for 
    /// saving, searching, and loading
    /// objects to/from a disk.
    /// </summary>
    public static class DiskItem
    {
        /// <summary>
        /// Searches files within a directory
        /// and returns the first object that
        /// matches search specifications.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="directoryPath">Directory.</param>
        /// <param name="determinant">Function which determines search behavior.</param>
        /// <param name="item">Found item.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Searches files within a directory
        /// and returns all objects that
        /// match search specifications.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="directoryPath">Directory.</param>
        /// <param name="determinant">Function which determines search behavior.</param>
        /// <param name="item">Found item.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Loads file, then deserializes it.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="filePath">File.</param>
        /// <param name="item">Loaded item.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Serializes an object, then saves it to a file.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="filePath">File.</param>
        /// <param name="item">Item to save.</param>
        /// <returns></returns>
        public static bool Save<T>(string filePath, in T item)
        {
            string json;
            var converted = ToJson(item, out json);
            if (!converted) return false;
            File.WriteAllText(filePath, json);
            return true;
        }
        /// <summary>
        /// Converts object to JSON.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="item">Object to serialize.</param>
        /// <param name="json">Serialized JSON.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Converts JSON to object.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="json">JSON to deserialize.</param>
        /// <param name="item">Deserialized object.</param>
        /// <returns></returns>
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
