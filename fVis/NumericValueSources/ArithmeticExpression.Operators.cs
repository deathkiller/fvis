using System.Runtime.CompilerServices;
using fVis.Callbacks;

namespace fVis.NumericValueSources
{
    public partial class ArithmeticExpression
    {
        /// <summary>
        /// Checks if callback function is used in the arithmetic expression
        /// </summary>
        /// <param name="callback">Callback name</param>
        /// <returns>Returns true if it's used; false, otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCallbackUsed(string callback)
        {
            return usedCallbacks.Contains(callback);
        }

        /// <summary>
        /// Returns precedence of given input string
        /// </summary>
        /// <param name="a">Input string</param>
        /// <returns>Precedence value</returns>
        private static int GetPrecedence(string a)
        {
            if (a == "+")
                return 1;
            if (a == "-")
                return 1;
            if (a == "*")
                return 2;
            if (a == "/" || a == "%")
                return 2;
            if (a == "^")
                return 3;
            if (char.IsLetter(a[0])) // Unary
                return 4;
            if (a == "(" || a == ")")
                return 5;

            return 0;
        }

        /// <summary>
        /// Checks if precedence of "b" is higher that "a"
        /// </summary>
        /// <param name="a">Input string A</param>
        /// <param name="b">Input string B</param>
        /// <returns>Returns true if it is; false, otherwise</returns>
        private static bool IsHigherPrecedence(string a, string b)
        {
            int ap = GetPrecedence(a);
            int bp = GetPrecedence(b);

            return (ap < bp);
        }

        /// <summary>
        /// Resolves operator by name
        /// </summary>
        /// <param name="name">Name of operator</param>
        /// <returns>Callback</returns>
        private OperatorCallbacks.OperatorFunction ResolveOperator(string name)
        {
            switch (name)
            {
                case "+":
                    usedCallbacks.Add("operator_add");
                    return callbacks.Add;
                case "-":
                    usedCallbacks.Add("operator_subtract");
                    return callbacks.Subtract;
                case "*":
                    usedCallbacks.Add("operator_multiply");
                    return callbacks.Multiply;
                case "/":
                    usedCallbacks.Add("operator_divide");
                    return callbacks.Divide;
                case "^":
                    usedCallbacks.Add("operator_pow");
                    return callbacks.Pow;
                case "%":
                    usedCallbacks.Add("operator_remainder");
                    return callbacks.Remainder;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Resolves operator by name
        /// </summary>
        /// <param name="name">Name of operator</param>
        /// <returns>Callback</returns>
        private OperatorCallbacks.OperatorUnaryFunction ResolveOperatorUnary(string name)
        {
            switch (name)
            {
                case "abs":
                    usedCallbacks.Add("operator_abs");
                    return callbacks.Abs;
                case "sqrt":
                    usedCallbacks.Add("operator_sqrt");
                    return callbacks.Sqrt;
                case "exp":
                    usedCallbacks.Add("operator_exp");
                    return callbacks.Exp;
                case "ln":
                    usedCallbacks.Add("operator_ln");
                    return callbacks.Ln;
                case "log":
                    usedCallbacks.Add("operator_log");
                    return callbacks.Log;
                case "sin":
                    usedCallbacks.Add("operator_sin");
                    return callbacks.Sin;
                case "cos":
                    usedCallbacks.Add("operator_cos");
                    return callbacks.Cos;
                case "tan":
                    usedCallbacks.Add("operator_tan");
                    return callbacks.Tan;
                case "asin":
                    usedCallbacks.Add("operator_asin");
                    return callbacks.Asin;
                case "acos":
                    usedCallbacks.Add("operator_acos");
                    return callbacks.Acos;
                case "atan":
                    usedCallbacks.Add("operator_atan");
                    return callbacks.Atan;
                case "sinh":
                    usedCallbacks.Add("operator_sinh");
                    return callbacks.Sinh;
                case "cosh":
                    usedCallbacks.Add("operator_cosh");
                    return callbacks.Cosh;
                case "tanh":
                    usedCallbacks.Add("operator_tanh");
                    return callbacks.Tanh;

                case "round":
                    usedCallbacks.Add("operator_round");
                    return callbacks.Round;
                case "floor":
                    usedCallbacks.Add("operator_floor");
                    return callbacks.Floor;
                case "ceil":
                    usedCallbacks.Add("operator_ceil");
                    return callbacks.Ceil;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Rewires callback pointers after callbacks change
        /// </summary>
        private void RewireCallbacks()
        {
            for (int i = 0; i < postfix.Length; i++) {
                Operator op;
                OperatorUnary opu;

                if ((op = postfix[i] as Operator) != null) {
                    op.Function = ResolveOperator(op.Name);
                } else if ((opu = postfix[i] as OperatorUnary) != null) {
                    opu.Function = ResolveOperatorUnary(opu.Name);
                }
            }
        }
    }
}