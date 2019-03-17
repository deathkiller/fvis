using System;
using System.Collections.Generic;
using System.Globalization;
using fVis.Callbacks;

namespace fVis.NumericValueSources
{
    /// <summary>
    /// Provides mechanism to parse arithmetic expression to mathematical function
    /// </summary>
    public partial class ArithmeticExpression : INumericValueSource
    {
        /// <summary>
        /// Parses input string to arithmetic expression
        /// </summary>
        /// <param name="input">Input string with infix representation</param>
        /// <returns>Arithmetic expression</returns>
        public static ArithmeticExpression Parse(string input)
        {
            return new ArithmeticExpression(input);
        }

        private enum PrecedingItem
        {
            Number,
            Operator,
            OperatorUnary,
            ParenthesesStart,
            ParenthesesEnd
        }

        private PostfixItem[] postfix;
        private string variableName;
        private OperatorCallbacks callbacks = new DotNetOperatorCallbacks();

        private readonly HashSet<string> usedCallbacks = new HashSet<string>();

        /// <summary>
        /// Callbacks used to compute value of the expression
        /// </summary>
        public OperatorCallbacks Callbacks
        {
            get { return callbacks; }
            set
            {
                callbacks = value;

                RewireCallbacks();
            }
        }

        /// <summary>
        /// Returns true if the expression contains only one item with constant value
        /// </summary>
        public bool IsSimpleConstantOnly
        {
            get { return (postfix.Length == 1 && postfix[0] is Constant); }
        }

        /// <summary>
        /// Returns name of variable used in the expression; or null
        /// </summary>
        public string VariableName
        {
            get { return variableName; }
        }

        private ArithmeticExpression(string input)
        {
            ParseToPostfix(input);

            ValidatePostfix();
        }

        public double Evaluate(double x)
        {
            PostfixItem[] copy = new PostfixItem[postfix.Length];
            postfix.CopyTo(copy, 0);

            int i;
            for (i = copy.Length - 1; i >= 0; i--) {
                Operator op;
                OperatorUnary opu;

                if ((op = copy[i] as Operator) != null) {
                    int i2 = FindFirstUsableIndex(copy, i);
                    int i1 = FindFirstUsableIndex(copy, i2);

                    // (i1 == -1 || i2 == -1) is already validated
                    
                    double value1 = GetValue(copy[i1], x);
                    double value2 = GetValue(copy[i2], x);

                    double value = op.Function(value1, value2);

                    copy[i] = new Constant {
                        Value = value
                    };

                    copy[i1] = null;
                    copy[i2] = null;
                } else if ((opu = copy[i] as OperatorUnary) != null) {
                    int i1 = FindFirstUsableIndex(copy, i);

                    // (i1 == -1) is already validated

                    double value1 = GetValue(copy[i1], x);

                    double value = opu.Function(value1);

                    copy[i] = new Constant {
                        Value = value
                    };

                    copy[i1] = null;
                }
            }

            while (++i < copy.Length) {
                Constant c;
                if ((c = copy[i] as Constant) != null) {
                    return c.Value;
                }

                if (copy[i] is Variable) {
                    return x;
                }
            }

            return double.NaN;
        }

        /// <summary>
        /// Validate postfix representation of the arithmetic expression
        /// </summary>
        private void ValidatePostfix()
        {
            PostfixItem[] copy = new PostfixItem[postfix.Length];
            postfix.CopyTo(copy, 0);

            int i;
            for (i = copy.Length - 1; i >= 0; i--) {
                if (copy[i] is Operator) {
                    int i2 = FindFirstUsableIndex(copy, i);
                    int i1 = FindFirstUsableIndex(copy, i2);

                    // Probably shouldn't happen...
                    if (i1 == -1 || i2 == -1) {
                        throw new FormatException();
                    }

                    copy[i] = new Constant();
                    copy[i1] = null;
                    copy[i2] = null;
                } else if (copy[i] is OperatorUnary) {
                    int i1 = FindFirstUsableIndex(copy, i);

                    // Probably shouldn't happen...
                    if (i1 == -1) {
                        throw new FormatException();
                    }

                    copy[i] = new Constant();
                    copy[i1] = null;
                }
            }

            int numberCount = 0;
            while (++i < copy.Length) {
                if (copy[i] is Constant || copy[i] is Variable) {
                    numberCount++;
                }
            }

            // Probably shouldn't happen...
            if (numberCount != 1) {
                throw new FormatException();
            }
        }

        /// <summary>
        /// Parse infix representation to postfix representation
        /// </summary>
        /// <param name="input">Infix representation of arithmetic expression</param>
        private unsafe void ParseToPostfix(string input)
        {
            variableName = null;

            Stack<PostfixItem> postfixStack = new Stack<PostfixItem>();
            Stack<PostfixItem> operatorsStack = new Stack<PostfixItem>();

            int level = 0;
            PrecedingItem preceding = PrecedingItem.ParenthesesStart;

            int length = input.Length;
            fixed (char* ptr = input) {
                for (int i = 0; i < length; i++) {
                    if (char.IsDigit(ptr[i]) || ptr[i] == '.' ||
                        ((preceding == PrecedingItem.Operator || preceding == PrecedingItem.ParenthesesStart) && (ptr[i] == '-' || ptr[i] == '+'))) {
                        // Number
                        if (preceding == PrecedingItem.Number || preceding == PrecedingItem.OperatorUnary || preceding == PrecedingItem.ParenthesesEnd) {
                            throw new SyntaxException(input, i, SyntaxException.Type.Unknown);
                        }

                        double number = ExtractNumber(ptr, length, ref i);
                        if (double.IsNaN(number)) {
                            throw new SyntaxException(input, i, SyntaxException.Type.InvalidNumber);
                        }

                        postfixStack.Push(new Constant {
                            Value = number
                        });

                        preceding = PrecedingItem.Number;
                    } else if (char.IsLetter(ptr[i])) {
                        // Function/constant/variable
                        string name = ExtractName(ptr, length, ref i);
                        OperatorCallbacks.OperatorUnaryFunction function = ResolveOperatorUnary(name);
                        if (function != null) {
                            if (preceding == PrecedingItem.OperatorUnary || preceding == PrecedingItem.ParenthesesEnd) {
                                throw new SyntaxException(input, i, SyntaxException.Type.Unknown);
                            }

                            // Insert multiply between number and function
                            if (preceding == PrecedingItem.Number) {
                                ParseOperator('*', ref preceding, postfixStack, operatorsStack);
                            }

                            operatorsStack.Push(new OperatorUnary {
                                Name = name,
                                Function = function
                            });

                            preceding = PrecedingItem.OperatorUnary;
                        } else {
                            if (preceding == PrecedingItem.OperatorUnary || preceding == PrecedingItem.ParenthesesEnd) {
                                throw new SyntaxException(input, i, SyntaxException.Type.Unknown);
                            }

                            // Insert multiply between number and constant/variable
                            if (preceding == PrecedingItem.Number) {
                                ParseOperator('*', ref preceding, postfixStack, operatorsStack);
                            }

                            if (name == "e" || name == "E") {
                                postfixStack.Push(new Constant {
                                    Value = callbacks.E
                                });
                            } else if (name == "pi" || name == "PI") {
                                postfixStack.Push(new Constant {
                                    Value = callbacks.PI
                                });
                            } else {
                                if (variableName != null && variableName != name) {
                                    throw new SyntaxException(input, i - name.Length + 1, SyntaxException.Type.DistinctVariableCountExceeded);
                                }

                                postfixStack.Push(new Variable());

                                variableName = name;
                            }

                            preceding = PrecedingItem.Number;
                        }
                    } else if (ptr[i] == '(') {
                        // Open parenthesis
                        if (preceding == PrecedingItem.Number || preceding == PrecedingItem.ParenthesesEnd) {
                            throw new SyntaxException(input, i, SyntaxException.Type.Unknown);
                        }

                        operatorsStack.Push(new InternalCommandBlock {
                            Value = "("
                        });

                        preceding = PrecedingItem.ParenthesesStart;
                        level++;
                    } else if (ptr[i] == ')') {
                        // Close parenthesis
                        if (preceding == PrecedingItem.Operator || preceding == PrecedingItem.OperatorUnary ||
                            preceding == PrecedingItem.ParenthesesStart) {
                            throw new SyntaxException(input, i, SyntaxException.Type.Unknown);
                        }

                        while (operatorsStack.Count > 0) {
                            InternalCommandBlock commandBlock = operatorsStack.Peek() as InternalCommandBlock;
                            if (commandBlock != null && commandBlock.Value == "(") {
                                break;
                            }

                            PostfixItem item = operatorsStack.Pop();
                            postfixStack.Push(item);
                        }

                        if (operatorsStack.Count == 0) {
                            throw new SyntaxException(input, i, SyntaxException.Type.ParenthesesCountMismatch);
                        }

                        operatorsStack.Pop();

                        preceding = PrecedingItem.ParenthesesEnd;
                        level--;
                    } else if (ptr[i] == '#') {
                        // Comment
                        break;
                    } else if (ptr[i] != ' ') {
                        // Operator
                        if (!ParseOperator(ptr[i], ref preceding, postfixStack, operatorsStack)) {
                            throw new SyntaxException(input, i, SyntaxException.Type.Unknown);
                        }
                    }
                }
            }

            // Check proper ending of arithmetic expression
            if (level > 0) {
                throw new SyntaxException(input, input.Length - 1, SyntaxException.Type.ParenthesesCountMismatch);
            }
            if (preceding == PrecedingItem.Operator || preceding == PrecedingItem.OperatorUnary || preceding == PrecedingItem.ParenthesesStart) {
                throw new SyntaxException(input, input.Length - 1, SyntaxException.Type.Unknown);
            }

            // Empty "operators" stack
            while (operatorsStack.Count > 0) {
                PostfixItem item = operatorsStack.Pop();
                postfixStack.Push(item);
            }

            postfix = postfixStack.ToArray();
        }

        /// <summary>
        /// Parses operator
        /// </summary>
        /// <param name="input">Input character</param>
        /// <param name="precedingItem">Preceding item type</param>
        /// <param name="postfixStack">Postfix stack</param>
        /// <param name="operatorsStack">Operators stack</param>
        /// <returns>Returns true if successful; false, otherwise</returns>
        private bool ParseOperator(char input, ref PrecedingItem precedingItem, Stack<PostfixItem> postfixStack, Stack<PostfixItem> operatorsStack)
        {
            string name = char.ToString(input);
            OperatorCallbacks.OperatorFunction function = ResolveOperator(name);
            if (function != null) {
                if (precedingItem == PrecedingItem.Operator || precedingItem == PrecedingItem.OperatorUnary ||
                    precedingItem == PrecedingItem.ParenthesesStart) {
                    return false;
                }

                while (operatorsStack.Count > 0) {
                    PostfixItem current = operatorsStack.Peek();

                    if (current is InternalCommandBlock) {
                        break;
                    }

                    Operator op;
                    if ((op = current as Operator) != null && IsHigherPrecedence(op.Name, name)) {
                        break;
                    }

                    OperatorUnary opu;
                    if ((opu = current as OperatorUnary) != null && IsHigherPrecedence(opu.Name, name)) {
                        break;
                    }

                    operatorsStack.Pop();
                    postfixStack.Push(current);
                }

                operatorsStack.Push(new Operator {
                    Name = name,
                    Function = function
                });

                precedingItem = PrecedingItem.Operator;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Find first usable postfix item
        /// </summary>
        /// <param name="items">List of items</param>
        /// <param name="i">Index to start</param>
        /// <returns>Index of found item</returns>
        private static int FindFirstUsableIndex(PostfixItem[] items, int i)
        {
            while (true) {
                i++;

                if (i >= items.Length || items[i] is Operator || items[i] is OperatorUnary) {
                    return -1;
                }
                if (items[i] is Constant || items[i] is Variable) {
                    break;
                }
            }

            return i;
        }

        /// <summary>
        /// Resolves value of given constant or variable
        /// </summary>
        /// <param name="item">Item to resolve</param>
        /// <param name="x">Value of variable</param>
        /// <returns>Value</returns>
        private static double GetValue(PostfixItem item, double x)
        {
            if (item is Variable) {
                return x;
            }

            Constant c;
            if ((c = item as Constant) != null) {
                return c.Value;
            }
            return double.NaN;
        }

        /// <summary>
        /// Extracts number from input
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">Length of string</param>
        /// <param name="offset">Offset in string</param>
        /// <returns>Extracted number</returns>
        private static unsafe double ExtractNumber(char* input, int length, ref int offset)
        {
            int start = offset;
            offset++;

            for (; offset < length; offset++) {
                if (char.IsDigit(input[offset]) || input[offset] == '.') {
                    continue;
                }

                if ((input[offset] == 'e' || input[offset] == 'E') && offset + 1 < length) {
                    if (char.IsDigit(input[offset + 1]) || input[offset + 1] == '-' || input[offset + 1] == '+') {
                        offset++;
                        continue;
                    }
                }

                break;
            }

            string substring = new string(input, start, offset - start);

            double result;
            if (substring == "+") {
                result = 1;
            } else if (substring == "-") {
                result = -1;
            } else if (!double.TryParse(substring, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) {
                result = double.NaN;
            }

            offset--;
            return result;
        }

        /// <summary>
        /// Extract function name from input
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">Length of string</param>
        /// <param name="offset">Offset in string</param>
        /// <returns>Function name</returns>
        private static unsafe string ExtractName(char* input, int length, ref int offset)
        {
            int start = offset;
            offset++;

            for (; offset < length; offset++) {
                if (!char.IsLetter(input[offset])) {
                    break;
                }
            }

            string result = new string(input, start, offset - start);
            offset--;
            return result;
        }
    }
}