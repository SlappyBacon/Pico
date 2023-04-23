using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Data
{
    /// <summary>
    /// This class allows you to load variables from a file, 
    /// use / modify them, and save them if desired.
    /// NOTE: Not all non-primitives types are supported.
    /// </summary>
    class DataFile
    {
        //Type, Name, Value
        Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>>();


        /// <summary>
        /// Set / change variable data.
        /// </summary>
        /// <typeparam name="T">Variable type.</typeparam>
        /// <param name="name">Name of variable.</param>
        /// <param name="value">Value of variable.</param>
        /// <param name="overwrite">Overwrite existing value?</param>
        /// <returns></returns>
        public bool SetVariable<T>(string name, T value, bool overwrite = false)
        {
            if (!data.ContainsKey(typeof(T))) data[typeof(T)] = new Dictionary<string, object>();   //Create new dictionary to store T variables
            if (!overwrite && data[typeof(T)].ContainsKey(name)) return false;  //Variable already exists
            data[typeof(T)][name] = value;
            return true;
        }
        /// <summary>
        /// Get variable data.
        /// </summary>
        /// <typeparam name="T">Variable type.</typeparam>
        /// <param name="name">Name of variable.</param>
        /// <returns></returns>
        public T GetVariable<T>(string name)
        {
            if (!data.ContainsKey(typeof(T))) throw new Exception("Variable doesn't exist.");
            if (!data[typeof(T)].ContainsKey(name)) throw new Exception("Variable doesn't exist");
            return (T)data[typeof(T)][name];
        }


        /// <summary>
        /// Delete variable data.
        /// </summary>
        /// <typeparam name="T">Variable type.</typeparam>
        /// <param name="name">Name of variable.</param>
        /// <returns></returns>
        public bool DeleteVariable<T>(string name)
        {
            if (!data.ContainsKey(typeof(T))) return false;         //No variables of that type stored
            if (!data[typeof(T)].ContainsKey(name)) return false;   //No variable with that name stored
            data[typeof(T)].Remove(name);                           //Remove variable
            if (data[typeof(T)].Count == 0) data.Remove(typeof(T)); //Removed the last variable of that type, remove that dictionary
            return true;
        }

        /// <summary>
        /// Save data to disk.
        /// </summary>
        /// <param name="path">Save file path.</param>
        /// <returns></returns>
        public bool Save(string path)
        {
            //if file exists, backup.  Overwrite old backup
            if (File.Exists(path)) File.Move(path, $"{path}.dbkp", true);

            throw new Exception();
            var lines = new string[0];

            //write data to file
            File.WriteAllLines(path, lines);
            return true;
        }
        public void Load()
        {
            throw new Exception();
        }
    }
}
