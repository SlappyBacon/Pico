using System;
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
        public static bool FloatIsInt(float number)
        {
            return !number.ToString().Contains('.');
        }


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
                string dividedAsString = divided.ToString();
                if (!dividedAsString.Contains('.')) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns if an integer is even.
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns></returns>
        public static bool IntIsEven(int number)
        {
            return number % 2 == 0;
        }



        /// <summary>
        /// Calculates the amount of each payment.
        /// </summary>
        /// <param name="loan">Loan amount.</param>
        /// <param name="annualInterestRate">Percent.</param>
        /// <param name="totalPayments">Total number of payments.</param>
        /// <param name="annualPayments">Number of payments per annua.</param>
        /// <returns>Payment total.</returns>
        public static double Finance(double loan, double annualInterestRate, int totalPayments = 12, int annualPayments = 12)
        {
            double paymentInterestRate = annualInterestRate / annualPayments;
            double paymentPrinciple = loan / totalPayments;
            double totalPaid = 0;
            for (int i = 0; i < totalPayments; i++)
            {
                double paymentInterestAmount = loan * paymentInterestRate;
                double payment = paymentPrinciple + paymentInterestAmount;
                loan -= paymentPrinciple;
                totalPaid += payment;
            }
            return totalPaid / totalPayments;
        }

    }
}
