using System;
using System.Text;
using System.Threading;

namespace Pico.Print
{
    public static class Print
    {
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
        public static void FakeLoading(string msg, int min = 5, int max = 50)
        {
            Console.WriteLine(msg);
            Console.WriteLine("0%            50%           100%");
            for (int i = 0; i < 32;)
            {
                Console.Write("▀");
                Thread.Sleep(Random.Shared.Next(min, max));
                i++;
            }
            Console.WriteLine();
        }
        public static void Alert(string text, ConsoleColor backgroundColor = ConsoleColor.Red)
        {
            ConsoleColor textColor;
            if (backgroundColor == ConsoleColor.Black || backgroundColor == ConsoleColor.DarkBlue) textColor = ConsoleColor.White;
            else textColor = ConsoleColor.Black;
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }


        public static void TimeStamp(string message)
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
            if (message != null && message != "")
            {
                sb.Append(' ');
                sb.Append(message);
            }
            Console.WriteLine(sb.ToString());
        }

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
