using System.Collections.Generic;
using System.Text;

namespace Pico.Args
{
    public static class ArgTools
    {
        public static string GetValue(string[] args, string find)
        {

            //Find find
            int found = IndexOf(args, find);
            if (found <= -1) return "";   //Not found
            if (found >= args.Length-1) return "";   //Found, but last arg

            //Check for start quote
            int start = found + 1;
            
            if (!args[start].StartsWith('\"')) return args[start];

            //find end quote
            int end = start;
            for (int i = start; i < args.Length;)
            {
                if (args[i].EndsWith('\"'))
                {
                    end = i;
                    break;
                }
                i++;
            }

            //Compile Result
            StringBuilder result = new StringBuilder();
            for (int i = start; i <= end;)
            {
                result.Append(args[i]);
                result.Append(' ');
                i++;
            }
            return result.ToString().Trim(' ').Trim('"');
        }


        public static int IndexOf(string[] args, string find)
        {
            for (int i = 0; i < args.Length;)
            {
                if (args[i] == find) return i;
                i++;
            }
            return -1;
        }
        /// <summary>
        /// Converts a string to string[]
        /// </summary>
        /// <param name="text">Text to split into args</param>
        /// <returns></returns>
        public static string[] StringToArgs(string text)
        {
            if (text == null) return null;
            string[] split = text.Split(' ');
            List<string> splitList = new List<string>(split.Length);
            for (int i = 0; i < split.Length;)
            {
                if (split[i] != "" && split[i] != null) splitList.Add(split[i]);
                i++;
            }
            return splitList.ToArray();
        }
        /// <summary>
        /// Converts a string[] to string
        /// </summary>
        /// <param name="args">Args to combine into a string</param>
        /// <returns></returns>
        public static string ArgsToString(string[] args)
        {
            //USE STRINGBUILDER HERE
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < args.Length - 1;)
            {
                text.Append(args[i]);
                text.Append(' ');
                i++;
            }
            return text.ToString().Trim(' ');
        }
    }
}
