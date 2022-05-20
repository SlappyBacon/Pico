using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Data
{
    struct DataFile
    {
        //Type, Name, Value
        static Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>>();

        public bool SetVariable<T>(string name, T value, bool overwrite = false)
        {
            if (!data.ContainsKey(typeof(T))) data[typeof(T)] = new Dictionary<string, object>();   //Create new dictionary to store T variables
            if (!overwrite && data[typeof(T)].ContainsKey(name)) return false;  //Variable already exists
            data[typeof(T)][name] = value;
            return true;
        }

        public T GetVariable<T>(string name)
        {
            if (!data.ContainsKey(typeof(T))) throw new Exception("Variable doesn't exist.");
            if (!data[typeof(T)].ContainsKey(name)) throw new Exception("Variable doesn't exist");
            return (T)data[typeof(T)][name];
        }

        public bool RemoveVariable<T>(string name)
        {
            if (!data.ContainsKey(typeof(T))) return false;         //No variables of that type stored
            if (!data[typeof(T)].ContainsKey(name)) return false;   //No variable with that name stored
            data[typeof(T)].Remove(name);                           //Remove variable
            if (data[typeof(T)].Count == 0) data.Remove(typeof(T)); //Removed the last variable of that type, remove that dictionary
            return true;
        }

        public bool Save(string path)
        {
            //if file exists, backup.  Overwrite old backup
            if (File.Exists(path)) File.Move(path, $"{path}.backup", true);

            //convert variables to text to write to file
            //type,name,value
            StringBuilder stringBuilder = new StringBuilder();
            List<string> fileLines = new List<string>();
            foreach (Type type in data.Keys)
            {
                foreach (string name in data[type].Keys)
                {
                    stringBuilder.Append(type.FullName);
                    stringBuilder.Append('■');
                    stringBuilder.Append(name);
                    stringBuilder.Append('■');
                    stringBuilder.Append(data[type][name]);
                    fileLines.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }

            //write data to file
            File.WriteAllLines(path, fileLines.ToArray());
            return true;
        }

        public bool Load(string path)
        {
            //check if file exists
            if (!File.Exists(path)) return false;

            //load file lines
            string[] fileLines = File.ReadAllLines(path);

            //create new data set
            Dictionary<Type, Dictionary<string, object>> newData = new Dictionary<Type, Dictionary<string, object>>();

            //populate data
            foreach (string line in fileLines)
            {
                //Try to get data from file line (TRY CATCH ALL?)
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
                    Console.WriteLine($"Failed to load data from: {line}");
                }
            }

            //once complete, replace the existing data set
            data = newData;

            return true;
        }

        //PARSE ABSTRACT OBJECTS
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


        //DEBUGGING
        public void DumpToConsole()
        {
            Console.WriteLine("---DUMP---");
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Type t in data.Keys)
            {
                foreach (string n in data[t].Keys)
                {
                    stringBuilder.Append(t);
                    stringBuilder.Append(" : ");
                    stringBuilder.Append(n);
                    stringBuilder.Append(" : ");
                    stringBuilder.Append(data[t][n]);
                    Console.WriteLine(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }
            Console.WriteLine("----------");
        }
    }
}
