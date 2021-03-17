using System;

namespace TestCalculator
{
    internal static class Program
    {
        static void Main()
        {
            do
            {
                Console.WriteLine("Enter expression:");

                var expression = Console.ReadLine();

                var calculator = new Calculator();
                var result = calculator.Calculate(expression);

                if (double.TryParse(result, out double validResult))
                {
                    ShowResult("Answer: " + Math.Round(validResult, 2));
                }
                else
                {
                    ShowResult(result);
                }

                
            } while (IsContinue());
        }
        
        private static bool IsContinue()
        {
            while (true)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Enter:
                        return true;
                    case ConsoleKey.Escape:
                        return false;
                }
            }
        }

        private static void ShowResult(string answer)
        {
            Console.WriteLine("********");
            Console.WriteLine(answer);
            Console.WriteLine("Press Enter to continue.");
            Console.WriteLine("Press ESC to exit.");
            Console.WriteLine("********");
        }
    }
}