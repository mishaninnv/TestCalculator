namespace TestCalculator
{
    public class Calculator
    {
        private string _result = "Not performed.";

        /// <summary>
        /// Calculates the transmitted expression according to RPN algorithm.
        /// </summary>
        /// <param name="expression">Expression to evaluate.</param>
        /// <returns>if the expression is correct: the result of the calculation. Otherwise: "Not performed". </returns>
        public string Calculate(string expression)
        {
            ParseExpression(expression);

            return _result;
        }

        /// <summary>
        /// Sequential call of methods to transform and calculate the result of an expression.
        /// </summary>
        /// <param name="expression">Expression to evaluate.</param>
        private void ParseExpression(string expression)
        {
            var mathExpression = new MathExpression(expression);

            if (!mathExpression.IsValid) return;

            var listOfElements = new ListOfElements(mathExpression);
            var rpnExpression = new RpnExpression(listOfElements);
            var rpnCalculationAlgorithm = new RpnCalculationAlgorithm(rpnExpression);

            if (rpnCalculationAlgorithm.IsValid)
                _result = rpnCalculationAlgorithm.Result;
        }
    }
}