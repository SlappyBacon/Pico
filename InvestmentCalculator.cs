namespace Pico.Calculators
{
    public static class InvestmentCalculator
    {
        /// <summary>
        /// Calculate how much you would have to rent something for to reach a certain target.
        /// </summary>
        /// <param name="investment">Cost of the item that's being rented.</param>
        /// <param name="termsOwned">Terms that the renal item is owned for.</param>
        /// <param name="targetOutcome">Percent of initial investment.  Eg: 1.25f == 25% profit</param>
        /// <param name="efficiency">Percent of terms that the item is rented for. Eg: 0.5f == 50%</param>
        /// <returns></returns>
        public static double CalculateRentalCost(double investment, int termsOwned, double targetOutcome = 1f, double efficiency = 1)
        {
            //Bounds
            if (efficiency <= 0) return 0;
            if (efficiency > 1) efficiency = 1;

            //Calc amount
            double amount = investment / termsOwned;

            //Modifiers
            if (targetOutcome != 1) amount *= targetOutcome;
            if (efficiency != 1) amount /= efficiency;
            return amount;
        }



        /// <summary>
        /// Calculates the amount of each payment.
        /// </summary>
        /// <param name="loan">Loan amount.</param>
        /// <param name="annualInterestRate">Percent.</param>
        /// <param name="totalPayments">Total number of payments.</param>
        /// <param name="annualPayments">Number of payments per annua.</param>
        /// <returns>Payment total.</returns>
        public static double CalculateFinancePayment(double loan, double annualInterestRate, int totalPayments = 12, int annualPayments = 12)
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
