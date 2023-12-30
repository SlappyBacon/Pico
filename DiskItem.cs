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
        /// Iterates through files within a directory
        /// and performs an action to each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directoryPath">Directory.</param>
        /// <param name="doThis">Action to perform.</param>
        public static async Task<int> ForEachItemAsync<T>(string directoryPath, Action<T, CancellationToken> doThis, CancellationToken ct)
        {
            int total = 0;
            await FileTools.ForEachFilePathAsync(directoryPath, DirectoryPathAction, false, true, ct);
            return total;
            void DirectoryPathAction(string filePath)
            {
                if (ct.IsCancellationRequested) return;

                T loadedItem;
                bool didLoad = Load(filePath, out loadedItem);
                if (!didLoad) return;
                doThis(loadedItem, ct);
                total++;
            }
        }
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

            bool found = false;
            T foundItem = default;

            CancellationTokenSource cts = new CancellationTokenSource();
            var task = ForEachItemAsync<T>(directoryPath, ItemAction, cts.Token);
            task.Wait();
            cts.Dispose();

            item = foundItem;
            return found;

            void ItemAction(T item, CancellationToken ct)
            {
                if (ct.IsCancellationRequested) return;

                if (!determinant(item)) return;
                found = true;
                foundItem = item;
                cts.Cancel();
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
        /// <returns></returns>
        public static List<T> LoadFindAll<T>(string directoryPath, Func<T, bool> determinant)
        {
            List<T> result = new List<T>();

            if (!Directory.Exists(directoryPath)) return result;

            var cts = new CancellationTokenSource();
            var task = ForEachItemAsync<T>(directoryPath, ItemAction, cts.Token);
            task.Wait();
            cts.Dispose();

            return result;

            void ItemAction(T item, CancellationToken ct)
            {
                if (ct.IsCancellationRequested) return;

                if (!determinant(item)) return;

                lock (result)
                {
                    result.Add(item);
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
