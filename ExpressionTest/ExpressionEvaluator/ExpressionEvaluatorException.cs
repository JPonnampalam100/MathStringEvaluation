using System;

namespace ExpressionEvaluator
{
    /// <summary>
    /// Custom Expression usd ny ExpressionEvaluator
    /// </summary>
    public class ExpressionEvaluatorException : Exception
    {
        public ExpressionEvaluatorException(string message) : base(message)
        {
        }
    }
}