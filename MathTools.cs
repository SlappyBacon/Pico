using System;
using System.Numerics;

namespace Pico.Maths
{
    public static class MathTools
    {


        //WRITE A CONSTRAIN METHOD
        //AND MIN/MAX METHODS




        public static bool IsNumber(object number)
        {
            if (number is int) return true;
            if (number is float) return true;
            if (number is double) return true;
            return false;
        }

        public static double MapDouble(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }
        public static bool FloatIsInt(float number)
        {
            return !number.ToString().Contains('.');
        }
        public static bool IntIsPrime(int num)
        {
            for (long i = 1; i < num - 1;)
            {
                i++;
                float divided = (float)num / i;
                string dividedAsString = divided.ToString();
                if (!dividedAsString.Contains('.')) return false;
            }
            return true;
        }
        public static bool IntIsEven(int number)
        {
            string numberAsText = number.ToString();
            char c = numberAsText[numberAsText.Length - 1];
            if (c == '0' || c == '2' || c == '4' || c == '6' || c == '8') return true;
            return false;
        }
        public static Vector3 RandomFibVector3()
        {
            return FibVector3(new Random().Next(0, int.MaxValue), int.MaxValue);
        }

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
    }
}
