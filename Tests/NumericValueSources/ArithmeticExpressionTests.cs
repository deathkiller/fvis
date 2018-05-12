using System;
using fVis.Callbacks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fVis.NumericValueSources.Tests
{
    [TestClass]
    public class ArithmeticExpressionTests
    {
        [TestMethod]
        public void Parse_AllOperators()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("0 + 1 - (2 * (3 / 4)) ^ (5 % 6)");
            Assert.IsNotNull(ae);

            Assert.IsTrue(ae.IsCallbackUsed("operator_add"));
            Assert.IsTrue(ae.IsCallbackUsed("operator_subtract"));
            Assert.IsTrue(ae.IsCallbackUsed("operator_multiply"));
            Assert.IsTrue(ae.IsCallbackUsed("operator_divide"));
            Assert.IsTrue(ae.IsCallbackUsed("operator_pow"));
            Assert.IsTrue(ae.IsCallbackUsed("operator_remainder"));

            Assert.IsFalse(ae.IsCallbackUsed("unknown"));
        }

        [TestMethod]
        public void Parse_AllFunctions()
        {
            string[] functions = {
                "abs", "sqrt", "exp", "ln", "log",
                "sin", "cos", "tan",
                "asin", "acos", "atan",
                "sinh", "cosh", "tanh",
                "round", "floor", "ceil"
            };

            string input = "";
            for (int i = 0; i < functions.Length; i++) {
                if (i > 0) {
                    input += " + ";
                }

                input += functions[i] + "(x)";
            }

            ArithmeticExpression ae = ArithmeticExpression.Parse(input);
            Assert.IsNotNull(ae);

            for (int i = 0; i < functions.Length; i++) {
                Assert.IsTrue(ae.IsCallbackUsed("operator_" + functions[i]));
            }

            Assert.IsFalse(ae.IsCallbackUsed("unknown"));
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Empty expression not allowed.")]
        public void Parse_Empty()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Only one variable name is allowed.")]
        public void Parse_Variables()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("x * y");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Unknown function specified.")]
        public void Parse_UnknownFunction()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("unknown(x)");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Inconsistent parentheses specified.")]
        public void Parse_BadParentheses_1()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("(1 + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Inconsistent parentheses specified.")]
        public void Parse_BadParentheses_2()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("((1 + 2)))");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid number format, dot should be used instead of comma.")]
        public void Parse_InvalidNumber()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("5,5");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_1()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("* 9");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_2()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("sin");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_3()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("55 +");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_4()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("44 */ 33");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_5()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("10 20 + 30");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_6()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("(2 +)");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_7()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("sin sin(x)");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_8()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("(5 * 5) sqrt(16)");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_9()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("(5 * 5) pi");
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException), "Invalid expression specified.")]
        public void Parse_InvalidInput_10()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("50 50");
        }


        [TestMethod]
        public void Evaluate_Comment()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("1 + 2 + 3 # This is comment!");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(6, result);
            Assert.IsNull(ae.VariableName);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Constant_1()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("-5");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(-5, result);
            Assert.IsNull(ae.VariableName);
            Assert.IsTrue(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Constant_2()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("1e2");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(100, result);
            Assert.IsNull(ae.VariableName);
            Assert.IsTrue(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Constant_3()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("6e-3");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(0.006, result);
            Assert.IsNull(ae.VariableName);
            Assert.IsTrue(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Constant_AutoMultiply()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("2pi");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(2.0 * Math.PI, result);
            Assert.IsNull(ae.VariableName);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_KnownConstants()
        {
            ArithmeticExpression ae1 = ArithmeticExpression.Parse("pi");
            double result1 = ae1.Evaluate(double.NaN);
            Assert.AreEqual(Math.PI, result1);

            ArithmeticExpression ae2 = ArithmeticExpression.Parse("e");
            double result2 = ae2.Evaluate(double.NaN);
            Assert.AreEqual(Math.E, result2);
        }

        [TestMethod]
        public void Evaluate_Variable()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("x");
            double result = ae.Evaluate(-10);
            Assert.AreEqual(-10, result);
            Assert.AreEqual("x", ae.VariableName);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_SimpleExpression()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("(2 - 1 + 6) % 5");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual((2 - 1 + 6) % 5, result);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Parentheses()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("((( ((( ((( 1 + 2 ))) + 3 ))) + 4 ))) + 5");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(1 + 2 + 3 + 4 + 5, result);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Precedence()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("1 / (2 * 4) ^ 2");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(1.0 / Math.Pow(2.0 * 4.0, 2), result);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_SimpleFunction()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("cos(custom)");
            double result = ae.Evaluate(0);
            Assert.AreEqual(1.0, result);
            Assert.AreEqual("custom", ae.VariableName);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_SimpleFunction_AutoMultiply()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("5sqrt(x)");
            double result = ae.Evaluate(16);
            Assert.AreEqual(5.0 * Math.Sqrt(16), result);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Variable_2()
        {
            ArithmeticExpression ae1 = ArithmeticExpression.Parse("sin(x)");
            double result1 = ae1.Evaluate(10);

            ArithmeticExpression ae2 = ArithmeticExpression.Parse("sin(10)");
            double result2 = ae2.Evaluate(double.NaN);

            Assert.AreEqual(result1, result2);
        }

        [TestMethod]
        public void Evaluate_Function_Log()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("log(10 * 10 * 10 * 10)");
            double result = ae.Evaluate(double.NaN);
            Assert.AreEqual(4.0, result);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Function_RoundUp()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("round(y)");
            double result = ae.Evaluate(6.51);
            Assert.AreEqual(7.0, result);
            Assert.AreEqual("y", ae.VariableName);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Function_RoundDown()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("round(z)");
            double result = ae.Evaluate(6.49);
            Assert.AreEqual(6.0, result);
            Assert.AreEqual("z", ae.VariableName);
            Assert.IsFalse(ae.IsSimpleConstantOnly);
        }

        [TestMethod]
        public void Evaluate_Callbacks()
        {
            ArithmeticExpression ae = ArithmeticExpression.Parse("(+1) * (-2) * 3");
            DotNetOperatorCallbacks callbacks = new DotNetOperatorCallbacks();

            ae.Callbacks = callbacks;

            double result = ae.Evaluate(0);
            Assert.AreEqual((+1) * (-2) * 3, result);
            Assert.AreEqual(callbacks, ae.Callbacks);

            Assert.IsFalse(callbacks.IsDisposed);

            callbacks.Dispose();

            Assert.IsTrue(callbacks.IsDisposed);
        }
    }
}