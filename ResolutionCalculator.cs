using System;
using Pico.Maths;

namespace Pico.Media
{
    public static class ResolutionCalculator
    {
        /// <summary>
        /// Lists all absolute resolutions within the parameters.
        /// </summary>
        /// <param name="ratio">Width / Height (Eg. 16f/9f)</param>
        /// <param name="widthMin">Minimum width.</param>
        /// <param name="widthMax">Maximum width.</param>
        public static void ListAllResolutions(float ratio, int widthMin, int widthMax)
        {
            for (int width = widthMin; width <= widthMax;)
            {
                float height = width / ratio;
                if (HeightIsValid(height)) Console.WriteLine($"{width} x {height}");
                width++;
            }
            bool HeightIsValid(float number)
            {
                //Check if number is an integer
                if (!MathTools.FloatIsInt(number)) return false;

                //Check if number is even
                if (!MathTools.IntIsEven((int)number)) return false;

                return true;
            }
        }
    }
}
