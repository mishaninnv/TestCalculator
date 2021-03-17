using System;
using System.Collections.Generic;
using static TestCalculator.MaterialLibrary;

namespace TestCalculator
{
    public class RpnCalculationAlgorithm
    {
        private readonly List<Element> _rpnExpression;
        private readonly Stack<Element> _elementsToCalculate;

        public string Result { get; private set; }

        public bool IsValid { get; private set; } = true;

        public RpnCalculationAlgorithm(RpnExpression rpnExpression)
        {
            _rpnExpression = rpnExpression.ExpressionToRpn;
            _elementsToCalculate = new Stack<Element>();

            Calculate();
        }

        /// <summary>
        /// Loops through the elements of the expression. Operands are pushed onto the stack for computation,
        /// for an operator, it takes the last two operands on the stack and performs a mathematical operation,
        /// pushing the result onto the stack for further computation. At the end, there is only one value left on
        /// the stack for calculation - the result.
        /// </summary>
        private void Calculate()
        {
            foreach (var element in _rpnExpression)
            {
                if (element.Type == EType.Operator)
                {
                    var rightOperand = _elementsToCalculate.Pop();
                    var leftOperand = _elementsToCalculate.Pop();

                    if (element.Value.Equals("/") && CheckRightOperandForZero(rightOperand)) return;

                    var result = Operations[element.Value](leftOperand.Value, rightOperand.Value);

                    _elementsToCalculate.Push(new Element {Type = EType.Operand, Value = result});
                }
                else
                {
                    _elementsToCalculate.Push(element);
                }
            }

            Result = _elementsToCalculate.Pop().Value;
        }

        /// <summary>
        /// Checks the right operand for equality to zero. 
        /// </summary>
        /// <param name="rightOperand">The right operand of a math expression.</param>
        /// <returns>If right operand is equal zero: true. Otherwise: false.</returns>
        private bool CheckRightOperandForZero(Element rightOperand)
        {
            if (rightOperand.Value.Equals("0"))
            {
                Console.WriteLine("Cannot be divided by zero.");
                IsValid = false;
                return true;
            }

            return false;
        }
    }
}