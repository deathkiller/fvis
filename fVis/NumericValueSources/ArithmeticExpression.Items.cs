using System;
using fVis.Callbacks;

namespace fVis.NumericValueSources
{
    public partial class ArithmeticExpression
    {
        private class PostfixItem
        {
        }

        private class InternalCommandBlock : PostfixItem
        {
            public string Value;
        }

        private class Constant : PostfixItem
        {
            public double Value;
        }

        private class Variable : PostfixItem
        {
        }

        private class Operator : PostfixItem
        {
            public string Name;
            public OperatorCallbacks.OperatorFunction Function;
        }

        private class OperatorUnary : PostfixItem
        {
            public string Name;
            public OperatorCallbacks.OperatorUnaryFunction Function;
        }
    }
}