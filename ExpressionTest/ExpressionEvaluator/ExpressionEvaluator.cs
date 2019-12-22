using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionEvaluator
{
    public static class ExpressionEvaluator
    {
        private static readonly string EMPTY_SPACE = " ";

        private static readonly char[] TokenOperands =
            {Operands.Add, Operands.Subtract, Operands.Multiply, Operands.Divide};
        /// <summary>
        /// Calculates the sum of a mathematical expression supplied in string form. Returns double
        /// </summary>
        /// <param name="sum">string containing math expression</param>
        /// <returns>the evaluated expression of "sum"</returns>
        public static double Calculate(string sum)
        {
            string postFix = InfixToPostFix(sum);
            double result = EvaluatePostFix(postFix);
            return result;
        }

        /// <summary>
        ///     This method converts Infix notation to Postfix notation as it is easier to work with operator precedence
        ///     Ex.  3 + 4 (Infix) => 3 4 + (postfix)
        ///     All operators are assumed to be binary operators. Unary operators ex ++ not supported
        /// </summary>
        /// <param name="expr">Infix math string expression</param>
        /// <returns>postfix string  of expr</returns>
        public static string InfixToPostFix(this string expr)
        {
            var postFixExpression = new StringBuilder();
            var ops = new Stack<char>();

            foreach (var token in ReadToken(expr.Trim()))
                switch (GetTokenType(token))
                {
                    case TokenType.Operand:
                        postFixExpression.Append(token + EMPTY_SPACE);
                        break;
                    case TokenType.LeftParentheses:
                        ops.Push(token[0]);
                        break;
                    case TokenType.RightParentheses:
                        ProcessRightParentheses(postFixExpression, ops);
                        break;
                    case TokenType.Operator:
                        ProcessOperator(postFixExpression, ops, token[0]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return CompletePostFixOp(postFixExpression, ops);
        }

        private static string CompletePostFixOp(StringBuilder postFixExpression, Stack<char> ops)
        {
            while (ops.Count > 0) postFixExpression.Append(ops.Pop());
            return postFixExpression.ToString();
        }

        private static void ProcessOperator(StringBuilder postFixExpression,
            Stack<char> ops, char operand)
        {
            if (ops.Count == 0 || ops.Peek() == Operands.LeftParentheses)
            {
                ops.Push(operand);
            }
            else
            {
                while (ops.Count > 0 && ops.Peek() != Operands.LeftParentheses && LessPrecedence(operand, ops.Peek()))
                    postFixExpression.Append(ops.Pop() + EMPTY_SPACE);

                ops.Push(operand);
            }
        }

        private static bool LessPrecedence(char op1, char op2)
        {
            if (op1 == op2)
            {
                return true;
            }
            if ((op1 != Operands.Multiply || op1 != Operands.Divide)
                && (op2 == Operands.Multiply || op1 == Operands.Divide))
                return true;
            return false;
        }

        private static void ProcessRightParentheses(StringBuilder postFixExpression,
            Stack<char> ops)
        {
            while (ops.Any() && ops.Peek() != Operands.LeftParentheses)
                postFixExpression.Append(ops.Pop() + EMPTY_SPACE);
            // Should have left parentheses remaining, so remove it
            if (ops.Count > 0) ops.Pop();
        }

        private static TokenType GetTokenType(string token)
        {
            if (double.TryParse(token, out _)) return TokenType.Operand;
            if (token.Length != 1) throw new ExpressionEvaluatorException($"Unknown token type {token}");
            if (token.IndexOfAny(TokenOperands) != -1) return TokenType.Operator;
            if (token == Operands.RightParentheses.ToString()) return TokenType.RightParentheses;
            if (token == Operands.LeftParentheses.ToString()) return TokenType.LeftParentheses;
            throw new ExpressionEvaluatorException($"Unknown token type {token}");
        }

        private static IEnumerable<string> ReadToken(string expr)
        {
            return expr.Split(' ');
        }

        public static double EvaluatePostFix(this string postFix)
        {
            var values = new Stack<double>();

            var args = postFix.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in args)
            {
                if (double.TryParse(token, out var value))
                {
                    values.Push(value);
                    continue;
                }

                var rhs = values.Pop();
                var lhs = values.Pop();
                var op = token[0];
                switch (op)
                {
                    case Operands.Add:
                        values.Push(lhs + rhs);
                        break;
                    case Operands.Subtract:
                        values.Push(lhs - rhs);
                        break;
                    case Operands.Multiply:
                        values.Push(lhs * rhs);
                        break;
                    case Operands.Divide:
                        values.Push(lhs / rhs);
                        break;
                }
            }

            return values.Pop();
        }
    }
}