using System.Collections.Generic;
using static TestCalculator.MaterialLibrary;

namespace TestCalculator
{
    public class RpnExpression
    {
        private readonly List<Element> _expressionOfElements;
        private readonly Stack<Element> _operatorStack;
        private Element _currentElement;

        public List<Element> ExpressionToRpn { get; }

        public RpnExpression(ListOfElements expressionOfElements)
        {
            _expressionOfElements = expressionOfElements.ExpressionOfElements;
            _operatorStack = new Stack<Element>();
            ExpressionToRpn = new List<Element>();

            ParseExpression();
        }

        /// <summary>
        /// Iterates over items to convert to rpn expression.
        /// </summary>
        private void ParseExpression()
        {
            foreach (var element in _expressionOfElements)
            {
                _currentElement = element;

                ElementHandler();
            }

            AddingUnrecordedOperators();
        }

        /// <summary>
        /// Gets type current element and if type is equal EType.Operand adds to expressionToRpn,
        /// else type is equal EType.Operator adds operator in stack.
        /// </summary>
        private void ElementHandler()
        {
            if (_currentElement.Type == EType.Operand)
            {
                ExpressionToRpn.Add(_currentElement);
            }
            else
            {
                AddOperatorToStack();
            }
        }

        /// <summary>
        /// If the operator stack is empty, add the operator, otherwise it calls the method for selecting the
        /// algorithm for adding the operator to the stack.
        /// </summary>
        private void AddOperatorToStack()
        {
            if (_operatorStack.Count <= 0)
            {
                _operatorStack.Push(_currentElement);
            }
            else
            {
                ChoiceOfAddingMethod();
            }
        }

        /// <summary>
        /// Calls the necessary method to add depending on the value of the operator.
        /// </summary>
        private void ChoiceOfAddingMethod()
        {
            switch (_currentElement.Value)
            {
                case "(":
                    _operatorStack.Push(_currentElement);
                    break;
                case ")":
                    AddingOperatorsBetweenBounds();
                    break;
                default:
                    AddingElementsWithHighPrecedence();
                    break;
            }
        }

        /// <summary>
        /// Writes all the statements contained on the stack between the parentheses in the RPN expression.
        /// </summary>
        private void AddingOperatorsBetweenBounds()
        {
            while (_operatorStack.Count > 0)
            {
                var currOperator = _operatorStack.Peek();

                if (OperatorValues[currOperator.Value] < OperatorValues[_currentElement.Value])
                {
                    _operatorStack.Pop();
                    return;
                }

                ExpressionToRpn.Add(_operatorStack.Pop());
            }
        }

        /// <summary>
        /// Compares the operators on the stack with the current operator. Operators with higher priority are written
        /// in rpn expression. Current operator add to stack.
        /// </summary>
        private void AddingElementsWithHighPrecedence()
        {
            while (_operatorStack.Count > 0)
            {
                var currOperator = _operatorStack.Peek();

                if (OperatorValues[currOperator.Value] < OperatorValues[_currentElement.Value])
                {
                    _operatorStack.Push(_currentElement);
                    return;
                }

                ExpressionToRpn.Add(_operatorStack.Pop());
            }

            _operatorStack.Push(_currentElement);
        }

        /// <summary>
        /// Adds all operators from the stack to the rpn expression.
        /// </summary>
        private void AddingUnrecordedOperators()
        {
            while (_operatorStack.Count > 0)
            {
                ExpressionToRpn.Add(_operatorStack.Pop());
            }
        }
    }
}