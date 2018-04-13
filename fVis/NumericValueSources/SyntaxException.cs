using System;

namespace fVis.NumericValueSources
{
    public class SyntaxException : Exception
    {
        public enum Type
        {
            Unknown,
            InvalidNumber,
            DistinctVariableCountExceeded,
            ParenthesesCountMismatch
        }

        public readonly string Input;
        public readonly int Index;
        public readonly Type ExceptionType;

        public SyntaxException(string input, int index, Type exceptionType)
        {
            Input = input;
            Index = index;
            ExceptionType = exceptionType;
        }
    }
}