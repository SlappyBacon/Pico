using System;
using System.Text;
using System.Threading;

namespace Pico.Print
{
    public static class Print
    {
        /// <summary>
        /// Prints with a cosmetic typewriter effect.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="pause">Pause time (in ms) at the end.</param>
        /// <param name="min">Minimum pause time (in ms) between keystrokes.</param>
        /// <param name="max">Maximum pause time (in ms) between keystrokes.</param>
        public static void TypeWrite(string text, int pause = 250, int min = 5, int max = 50)
        {
            for (int i = 0; i < text.Length;)
            {
                Console.Write(text[i]);
                Thread.Sleep(Random.Shared.Next(min, max));
                i++;
            }
            Console.WriteLine();
            Thread.Sleep(pause);
        }


        /// <summary>
        /// Prints a cosmetic loading effect.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="min">Minimum pause time (in ms) between ticks.</param>
        /// <param name="max">Maximum pause time (in ms) between ticks.</param>
        public static void FakeLoading(int min = 5, int max = 250)
        {
            //Rework with one-line buffer?
            Console.WriteLine("0%            50%           100%");
            for (int i = 0; i < 32;)
            {
                Console.Write("▀");
                Thread.Sleep(Random.Shared.Next(min, max));
                i++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prints a highlighted alert.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="color">Highlight color.</param>
        public static void Alert(string text, ConsoleColor color = ConsoleColor.Red)
        {
            ConsoleColor textColor;
            if (color == ConsoleColor.Black || color == ConsoleColor.DarkBlue) textColor = ConsoleColor.White;
            else textColor = ConsoleColor.Black;
            Console.BackgroundColor = color;
            Console.ForegroundColor = textColor;
            Console.WriteLine(text);
            Console.ResetColor();

            //Add beep boop beep?\\

        }


        /// <summary>
        /// Prints text with a timestamp.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public static void TimeStamp(string text)
        {
            //$"[{time}] {message}"
            DateTime now = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(now.Hour);
            sb.Append(':');
            sb.Append(now.Minute);
            sb.Append(':');
            sb.Append(now.Second);
            sb.Append(':');
            sb.Append(now.Millisecond);
            sb.Append(']');
            if (text != null && text != "")
            {
                sb.Append(' ');
                sb.Append(text);
            }
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Prints a byte array.
        /// </summary>
        /// <param name="bytes">Bytes to print.</param>
        public static void Array(byte[] bytes) //Add more types?
        {
            if (bytes == null)
            {
                Console.WriteLine("[null]");
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < bytes.Length;)
            {
                if (i < bytes.Length - 1) sb.Append($"{bytes[i]},");
                else sb.Append(bytes[i]);
                i++;
            }
            sb.Append(']');
            Console.WriteLine(sb.ToString());
        }
        /// <summary>
        /// Prints an int array.
        /// </summary>
        /// <param name="nums">Ints to print.</param>
        public static void Array(int[] nums)
        {
            if (nums == null)
            {
                Console.WriteLine("[null]");
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < nums.Length;)
            {
                if (i < nums.Length - 1) sb.Append($"{nums[i]},");
                else sb.Append(nums[i]);
                i++;
            }
            sb.Append(']');
            Console.WriteLine(sb.ToString());
        }
    }
}
