using System.Collections.Generic;
using System.Text;
using static TestCalculator.MaterialLibrary;

namespace TestCalculator
{
    public class ListOfElements
    {
        private readonly StringBuilder _expression;
        private readonly StringBuilder _operandBuffer;

        public List<Element> ExpressionOfElements { get; }

        public ListOfElements(MathExpression mathExpression)
        {
            _expression = mathExpression.GetExpression;
            ExpressionOfElements = new List<Element>();
            _operandBuffer = new StringBuilder();

            ParseExpression();
        }

        /// <summary>
        /// Defines the type of each character. Calls the desired method according to the type of the symbol.
        /// Adds unwritten values to the expression.
        /// </summary>
        private void ParseExpression()
        {
            for (var i = 0; i < _expression.Length; i++)
            {
                var currentSymbol = _expression[i];
                var symbolType = SymbolTypeDetection(currentSymbol);

                if (symbolType == EType.Operand)
                {
                    AddSymbolToOperandBuffer(currentSymbol);
                }
                else
                {
                    OperatorHandler(currentSymbol);
                }
            }

            if (!OperandBufferIsEmpty())
            {
                AddOperandToList();
            }
        }

        /// <summary>
        /// Determining the type of the transmitted symbol.
        /// </summary>
        /// <param name="symbol">Symbol which type need to know.</param>
        /// <returns>If symbol is contain in OperatorValues: EType.Operator. Otherwise: EType.Operand.</returns>
        private EType SymbolTypeDetection(char symbol)
        {
            return OperatorValues.ContainsKey(symbol.ToString())
                ? EType.Operator
                : EType.Operand;
        }

        /// <summary>
        /// Adding operand symbols to the buffer.
        /// </summary>
        /// <param name="symbol">The character to add to the buffer.</param>
        private void AddSymbolToOperandBuffer(char symbol)
        {
            _operandBuffer.Append(symbol);
        }

        /// <summary>
        /// If present, write the operand to the expression. Writing to operator expression.
        /// </summary>
        /// <param name="symbol">Operator symbol.</param>
        private void OperatorHandler(char symbol)
        {
            if (!OperandBufferIsEmpty())
            {
                AddOperandToList();
            }

            var newOperator = GetElement(EType.Operator, symbol.ToString());
            ExpressionOfElements.Add(newOperator);
        }

        /// <summary>
        /// Checks for the presence of an operand in the buffer.
        /// </summary>
        /// <returns>If buffer is empty: true. Otherwise: false.</returns>
        private bool OperandBufferIsEmpty()
        {
            return _operandBuffer.Length <= 0;
        }

        /// <summary>
        /// Adding a value from a buffer to an expression.
        /// </summary>
        private void AddOperandToList()
        {
            var newOperand = GetElement(EType.Operand, _operandBuffer.ToString());
            ExpressionOfElements.Add(newOperand);
            _operandBuffer.Clear();
        }

        private Element GetElement(EType type, string value)
        {
            return new Element {Type = type, Value = value};
        }
    }
}