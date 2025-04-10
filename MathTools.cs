using System.Numerics;

namespace Pico.Maths
{
    public static class MathTools
    {


        //WRITE A CONSTRAIN METHOD
        //AND MIN/MAX METHODS



        /// <summary>
        /// Returns if an object is a number.
        /// </summary>
        /// <param name="number">Object to check.</param>
        /// <returns></returns>
        public static bool IsNumber(object number)
        {
            if (number is int) return true;
            if (number is float) return true;
            if (number is double) return true;
            return false;
        }

        /// <summary>
        /// Map a double.
        /// </summary>
        /// <param name="value">Value to map.</param>
        /// <param name="fromLow">From-low value.</param>
        /// <param name="fromHigh">From-high value.</param>
        /// <param name="toLow">To-low value.</param>
        /// <param name="toHigh">To-high value.</param>
        /// <returns></returns>
        public static double MapDouble(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }


        /// <summary>
        /// Returns if a float has a decimal point.
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns></returns>
        public static bool FloatIsInt(float number) => number % 1 == 0;


        /// <summary>
        /// Returns if an integer is prime.
        /// </summary>
        /// <param name="num">Number to check.</param>
        /// <returns></returns>
        public static bool IntIsPrime(int num)
        {
            for (long i = 1; i < num - 1;)
            {
                i++;
                float divided = (float)num / i;
                if (FloatIsInt(divided)) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns if an integer is even.
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns></returns>
        public static bool IntIsEven(int number) => number % 2 == 0;



        /// <summary>
        /// Returns the Fibonacci Vector3, according
        /// to the given paramaters.
        /// </summary>
        /// <returns></returns>
        public static Vector3 FibVector3(int point, int points)
        {
            var k = point + .5f;

            var phi = Math.Acos(1f - 2f * k / points);
            var theta = Math.PI * (1 + Math.Sqrt(5)) * k;

            var x = Math.Cos(theta) * Math.Sin(phi);
            var y = Math.Sin(theta) * Math.Sin(phi);
            var z = Math.Cos(phi);

            return new Vector3((float)x, (float)y, (float)z);
        }


        static List<int> PrimesBetween(int lowerLimit, int upperLimit)
        {
            //Ensure all parameters are valid
            //Limits must both be positive integers
            //Upper limit must be larger than lower limit
            if (lowerLimit <= 0 || upperLimit <= 0 || upperLimit <= lowerLimit) return null;

            //Populate list with all numbers in between lower and upper limits
            List<int> result = new List<int>();
            for (int i = lowerLimit; i <= upperLimit; i++) result.Add(i);

            //Remove 1 if it's present
            result.Remove(1);

            int index = 0;
            while (true)
            {
                //Check to see if at end of list
                int number;
                try
                {
                    number = result[index];
                }
                catch
                {
                    break;
                }

                //Check if number is prime
                if (!IntIsPrime(number))
                {
                    //Remove all multiples of that number
                    int steps = number;
                    while (true)
                    {
                        if (number > upperLimit) break;
                        result.Remove(number);
                        number += steps;
                    }
                }
                else index++;
            }
            return result;
        }
    }
}
