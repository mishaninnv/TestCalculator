namespace TestCalculator
{
    public class Calculator
    {
        private string _result = "Not performed.";
        
        public string Calculate(string expression)
        {
            ParseExpression(expression);

            return _result;
        }

        private void ParseExpression(string expression)
        {
            var mathExpression = new MathExpression(expression);

            if (mathExpression.IsValid)
            {
                // can make future actions
            }
        }
        

    }
}