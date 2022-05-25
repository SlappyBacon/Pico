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
        static Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>>();


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
            if (File.Exists(path)) File.Move(path, $"{path}.backup", true);

            var lines = ToLines();

            //write data to file
            File.WriteAllLines(path, lines);
            return true;
        }
        /// <summary>
        /// convert variables to text to write to file
        /// type,name,value
        /// </summary>
        /// <returns></returns>
        public string[] ToLines()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<string> lines = new List<string>();
            foreach (Type type in data.Keys)
            {
                foreach (string name in data[type].Keys.ToArray())
                {
                    stringBuilder.Append(type.FullName);
                    stringBuilder.Append('■');
                    stringBuilder.Append(name);
                    stringBuilder.Append('■');
                    stringBuilder.Append(data[type][name]);
                    lines.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }
            return lines.ToArray();
        }
        /// <summary>
        /// Load data from disk.
        /// </summary>
        /// <param name="path">Load file path.</param>
        /// <returns></returns>
        public bool LoadFromFile(string path)
        {
            //check if file exists
            if (!File.Exists(path)) return false;

            //load file lines
            string[] lines = File.ReadAllLines(path);

            return LoadFromLines(lines);
        }
        /// <summary>
        /// Load data from disk.
        /// </summary>
        /// <param name="lines">Load file path.</param>
        /// <returns></returns>
        public bool LoadFromLines(string[] lines)
        {
            //create new data set
            Dictionary<Type, Dictionary<string, object>> newData = new Dictionary<Type, Dictionary<string, object>>();

            //populate data
            foreach (string line in lines)
            {
                //Try to get data from file line
                try
                {
                    //Find splits
                    int firstSplit = line.IndexOf("■");
                    int secondSplit = line.IndexOf("■", firstSplit + 1);

                    //Parse Type
                    string typeText = line.Substring(0, firstSplit);
                    Type type = Type.GetType(typeText);

                    //Parse Name
                    string name = line.Substring(firstSplit + 1, secondSplit - firstSplit - 1);

                    //Parse Value
                    string valueText = line.Substring(secondSplit + 1);
                    object value = TextToType(valueText, type);


                    //Add to new data set
                    if (!newData.ContainsKey(type)) newData[type] = new Dictionary<string, object>();
                    newData[type][name] = value;
                }
                catch
                {
                    
                }
            }

            //once complete, replace the existing data set
            data = newData;

            return true;
        }









        #region Non-Primitives
        public object TextToType(string text, Type toType)
        {
            switch (toType.Name)
            {
            //Insert special cases here
            case "Vector2":
                return TextToVector2(text);
            case "Vector3":
                return TextToVector3(text);
            default:
                return Convert.ChangeType(text, toType);
            }
        }
        public Vector2 TextToVector2(string text)
        {
            string[] split = text.Trim('<').Trim('>').Replace(" ", "").Split(',');
            float x = float.Parse(split[0]);
            float y = float.Parse(split[1]);
            return new Vector2(x, y);
        }
        public Vector3 TextToVector3(string text)
        {
            string[] split = text.Trim('<').Trim('>').Replace(" ", "").Split(',');
            float x = float.Parse(split[0]);
            float y = float.Parse(split[1]);
            float z = float.Parse(split[2]);
            return new Vector3(x, y, z);
        }
        #endregion
    }
}
