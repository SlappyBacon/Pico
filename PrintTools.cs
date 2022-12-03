using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Pico.Print
{
    public static class PrintTools
    {
        


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

        public static string FormatPercent(double num)
        {
            //1 equals 100%
            num *= 100;
            return Math.Round(num, 2).ToString() + "%";
        }

        public static string FormatDollars(double dollars)
        {
            return "$" + Math.Round(dollars, 2);
        }





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
        /// Read text from file, clear the console,
        /// then perform the animation.
        /// </summary>
        /// <param name="path">Text file path.</param>
        public static void TypeWriteFileRandom(string path)
        {
            if (!File.Exists(path)) using (var stream = File.Create(path)) stream.Dispose();
            string[] lines = File.ReadAllLines(path);

            //Character index of each line, starting at 0
            int[] lineIndex = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++) lineIndex[i] = 0;

            if (LinesRemaining() == 0) return;

            int msDelay = 100 / lines.Length;
            Random rnd = new Random();
            while (true)
            {
                var rndLine = rnd.Next(0, lines.Length);
                if (lineIndex[rndLine] == lines[rndLine].Length) continue;

                Console.SetCursorPosition(lineIndex[rndLine], rndLine);
                Console.Write(lines[rndLine][lineIndex[rndLine]]);
                lineIndex[rndLine]++;

                //Longer than buffer...
                if (lineIndex[rndLine] == Console.BufferWidth) lineIndex[rndLine] = lines[rndLine].Length;

                //All done
                if (LinesRemaining() == 0) break;

                Thread.Sleep(msDelay);
            }



            Console.Beep(1000, 100);
            Console.ReadLine();


            int LinesRemaining()
            {
                int count = 0;
                for (int i = 0; i < lines.Length; i++) if (lineIndex[i] != lines[i].Length) count++;
                return count;
            }
        }


        /// <summary>
        /// Prints message, then waits
        /// for [enter] to be pressed.
        /// </summary>
        /// <param name="action">Action to be displayed in message.</param>
        /// <returns></returns>
        public static void EnterToContinue(string action = "continue")
        {
            Console.WriteLine($"Press [enter] to {action.ToLower()}...");
            Console.ReadLine();
        }
    }
}
