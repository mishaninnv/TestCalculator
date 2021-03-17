using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static TestCalculator.MaterialLibrary;

namespace TestCalculator
{
    public class MathExpression
    {
        private StringBuilder _expression;
        private readonly char _separator;
        private int _parenthesesCounter;
        private int _indexOfCurrentSymbol;
        private char _previousSymbol;
        private char _currentSymbol;
        private char _nextSymbol;

        /// <summary>
        /// A state indicating the passing of all checks by the given expression.
        /// </summary>
        public bool IsValid { get; private set; } = true;

        public StringBuilder GetExpression => _expression;

        private bool IsNullOrEmpty => string.IsNullOrEmpty(_expression.ToString());

        public MathExpression(string expression)
        {
            _expression = new StringBuilder(expression);
            _separator = GetSeparatorOfCurrentLocale();

            ParseExpression();
        }

        /// <summary>
        /// Calls the methods needed to check and modify the expression.
        /// </summary>
        private void ParseExpression()
        {
            if (CheckIsNullOrEmpty()) return;
            if (!CheckClosingParentheses()) return;
            if (!CheckStartSymbolIsCorrect()) return;

            RemoveSpacesInExpression();
            SetSeparatorInExpression();
            AddSymbolToBegin();
            ParseEachSymbolOfExpression();
        }

        /// <summary>
        /// If expression is null or empty changes state IsValid = false, and show message.
        /// </summary>
        /// <returns>If expression is null: true. Otherwise: false.</returns>
        private bool CheckIsNullOrEmpty()
        {
            if (!IsNullOrEmpty) return false;

            ExpressionErrorHandler("Nothing entered");
            return true;
        }

        /// <summary>
        /// Checks the number of open and closed parentheses.
        /// </summary>
        /// <returns>If the number of open is equal to the number of closed: true. Otherwise: false.</returns>
        private bool CheckClosingParentheses()
        {
            var countLeftBounds = Regex.Matches(_expression.ToString(), @"\(");
            var countRightBounds = Regex.Matches(_expression.ToString(), @"\)");

            if (countLeftBounds.Count == countRightBounds.Count) return true;

            ExpressionErrorHandler("Not all parentheses are closed");
            return false;
        }

        /// <summary>
        /// Checks the contents of the starting symbol in the correct starting symbols. If not, a message is displayed.
        /// </summary>
        /// <returns>If first symbol is correct: true. Otherwise: false.</returns>
        private bool CheckStartSymbolIsCorrect()
        {
            var startSymbol = _expression[0];

            if (ContentOfSymbolInList(startSymbol, SymbolsToStart)) return true;

            ExpressionErrorHandler($"Invalid start symbol: {startSymbol}");
            return false;
        }

        /// <summary>
        /// Removes all spaces in expression.
        /// </summary>
        private void RemoveSpacesInExpression()
        {
            _expression = _expression.Replace(" ", "");
        }

        /// <summary>
        /// Changes all occurrences of "," or "." to the separator.
        /// </summary>
        private void SetSeparatorInExpression()
        {
            _expression = _separator.Equals('.')
                ? _expression.Replace(',', '.')
                : _expression.Replace('.', ',');
        }

        /// <summary>
        /// If expression begin with special symbols (+, -, separator), adds in begin 0.
        /// </summary>
        private void AddSymbolToBegin()
        {
            var startSymbol = _expression[0];

            if (ContentOfSymbolInList(startSymbol, SpecialSymbolsToStart))
                InsertSymbolToExpression(0, "0");
        }

        /// <summary>
        /// Takes each character to be tested against the conditions of the expression and,
        /// if necessary, add new characters to the expression.
        /// </summary>
        private void ParseEachSymbolOfExpression()
        {
            for (var i = 0; i < _expression.Length; i++)
            {
                _indexOfCurrentSymbol = i;
                _currentSymbol = _expression[i];

                if (i > 0) _previousSymbol = _expression[i - 1];
                if (i < _expression.Length - 1) _nextSymbol = _expression[i + 1];

                if (!SymbolValidation()) return;

                if (CheckNeedToAddSymbolBeforeSeparator())
                    InsertSymbolToExpression(i, "0");

                ParenthesisHandler();

                if (CheckSymbolSequence()) return;

                if (IsValid == false) return;
            }
        }

        /// <summary>
        /// Checks for a symbol in the list of valid symbols for the expression.
        /// </summary>
        /// <returns>If the symbol is contains: true. Otherwise: false. </returns>
        private bool SymbolValidation()
        {
            if (ContentOfSymbolInList(_currentSymbol, ValidSymbols)) return true;

            ExpressionErrorHandler($"Invalid symbol: {_expression[_indexOfCurrentSymbol]}");
            return false;
        }

        /// <summary>
        /// Checks the _current symbol for equality _separator and the contents of the _previousSymbol in the
        /// SymbolBeforeSeparator to determine whether to add a 0 before the separator.
        /// </summary>
        /// <returns>
        /// If _current symbol equals _separator and _previousSymbol contains a list: true.
        /// Otherwise: false.
        /// </returns>
        private bool CheckNeedToAddSymbolBeforeSeparator()
        {
            return _currentSymbol.Equals(_separator) && !ContentOfSymbolInList(_previousSymbol, SymbolsBeforeSeparator);
        }

        /// <summary>
        /// If the _currentSymbol is parentheses, then the methods needed to process them are called.
        /// </summary>
        private void ParenthesisHandler()
        {
            if (_currentSymbol.Equals('('))
            {
                ParsePreviousSymbolOpenParenthesis();
                ParseNextSymbolOpenParenthesis();

                _parenthesesCounter++;
            }
            else if (_currentSymbol.Equals(')'))
            {
                _parenthesesCounter--;

                if (_parenthesesCounter < 0)
                {
                    ExpressionErrorHandler("Wrong ordering of parentheses.");
                    return;
                }

                if (!CheckPreviousSymbolForCorrect()) return;

                CheckAndAddSymbolAfterCloseParenthesis();
            }
        }

        /// <summary>
        /// Checks:
        /// - that the _currentSymbol is not the first character of the expression;
        /// - the previous symbol for correctness;
        /// - if a symbol is added before the parenthesis.
        /// </summary>
        private void ParsePreviousSymbolOpenParenthesis()
        {
            if (_indexOfCurrentSymbol > 0)
            {
                if (!CheckPreviousSymbolForCorrect()) return;

                if (ContentOfSymbolInList(_previousSymbol, SymbolsBeforeOpenParenthesis))
                    InsertSymbolToExpression(_indexOfCurrentSymbol, "*");
            }
        }

        /// <summary>
        /// Checks if the previous symbol comes before the parenthesis.
        /// </summary>
        /// <returns>
        /// If _currentSymbol is '(' and _previousSymbol is not equal to _separator: true;
        /// or _currentSymbol is ')' and _previousSymbol is not contains in the ProhibitedSymbolsBeforeCloseBound: true;
        /// Otherwise: false.
        /// </returns>
        private bool CheckPreviousSymbolForCorrect()
        {
            switch (_currentSymbol)
            {
                case '(' when !_previousSymbol.Equals(_separator):
                case ')' when !ContentOfSymbolInList(_previousSymbol, ProhibitedSymbolsBeforeCloseBound):
                    return true;
            }

            ExpressionErrorHandler($"Invalid symbols sequence: {_previousSymbol} and {_currentSymbol}");
            return false;
        }

        /// <summary>
        /// Checks if the symbol is the last in expression and if "*" needs to be added after the closing parenthesis.
        /// </summary>
        private void CheckAndAddSymbolAfterCloseParenthesis()
        {
            if (_expression.Length > _indexOfCurrentSymbol + 1 &&
                ContentOfSymbolInList(_nextSymbol, SpecialSymbolsAfterCloseBound))
            {
                InsertSymbolToExpression(_indexOfCurrentSymbol + 1, "*");
            }
        }

        /// <summary>
        /// Checks _nextSymbol on correct and needed to add 0 after open parenthesis.
        /// </summary>
        private void ParseNextSymbolOpenParenthesis()
        {
            if (ContentOfSymbolInList(_nextSymbol, ProhibitedSymbolsAfterOpenBound))
            {
                ExpressionErrorHandler($"Invalid symbols sequence: {_currentSymbol} and {_nextSymbol}");
                return;
            }

            if (ContentOfSymbolInList(_nextSymbol, SpecialSymbolsAfterOpenParenthesis))
                InsertSymbolToExpression(_indexOfCurrentSymbol + 1, "0");
        }

        /// <summary>
        /// Checks for an invalid sequence of symbols.
        /// </summary>
        /// <returns>
        /// If _previousSymbol and _currentSymbol are contained in ConsistentSymbols: false. Otherwise: true.
        /// </returns>
        private bool CheckSymbolSequence()
        {
            if (ContentOfSymbolInList(_previousSymbol, ConsistentSymbols) &&
                ContentOfSymbolInList(_currentSymbol, ConsistentSymbols))
            {
                ExpressionErrorHandler($"Invalid order of elements: {_previousSymbol} and {_currentSymbol}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts the given symbol into the expression at the given index.
        /// </summary>
        /// <param name="index">The place where you want to insert the symbol.</param>
        /// <param name="symbol">Symbol to insert.</param>
        private void InsertSymbolToExpression(int index, string symbol)
        {
            _expression.Insert(index, symbol);
        }

        /// <summary>
        /// Checks the contents of a symbol in the list.
        /// </summary>
        /// <param name="symbol">Symbol of interest.</param>
        /// <param name="list">Search list.</param>
        /// <returns>If the list contains the symbol: true. Otherwise: false.</returns>
        private bool ContentOfSymbolInList(char symbol, string list)
        {
            return list.Contains(symbol);
        }

        private char GetSeparatorOfCurrentLocale()
        {
            return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
        }

        /// <summary>
        /// Sets the IsValid state to false. And shows the error message in the console.
        /// </summary>
        /// <param name="message">Console message.</param>
        private void ExpressionErrorHandler(string message)
        {
            Console.WriteLine(message);
            IsValid = false;
        }
    }
}