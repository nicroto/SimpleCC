using SimpleC.Tokens;
using System.Collections.Generic;

namespace SimpleC
{
    class Parser
    {
        private Scanner scanner;
        private Diagnostics diag;
        private Token token;

        public Program Result = new Program();

        public Parser(Scanner scanner, Diagnostics diag)
        {
            this.scanner = scanner;
            this.diag = diag;
        }

        public bool Parse()
        {
            ReadNextToken();
            return IsProgram() && CheckIsEndOfFIle();
        }

        private void Error(string message)
        {
            diag.Error(token.line, token.column, message);
            SkipUntilSemiColon();
        }

        void SkipUntilSemiColon()
        {
            Token Tok;
            do
            {
                Tok = scanner.Next();
            } while (!(
                (Tok is EOFToken) ||
                (Tok is SpecialSymbolToken) &&
                ((Tok as SpecialSymbolToken).value == ";"))
            );
        }

        public void ReadNextToken()
        {
            token = scanner.Next();
        }

        private bool CheckKeyword(string keyword)
        {
            bool result = (token is KeywordToken) && ((KeywordToken)token).value == keyword;
            if (result) ReadNextToken();
            return result;
        }

        private bool CheckIsIdent()
        {
            var result = token is IdentToken;
            if (result) ReadNextToken();
            return result;
        }

        private bool CheckSpecialSymbol(string symbol)
        {
            bool result = (token is SpecialSymbolToken) && ((SpecialSymbolToken)token).value == symbol;
            if (result) ReadNextToken();
            return result;
        }

        private bool CheckIsEndOfFIle()
        {
            return token is EOFToken;
        }

        /**
         * Grammar
         */

        // [1] Program = {Statement}
        public bool IsProgram()
        {
            while (!CheckIsEndOfFIle() && IsStatement(this.Result)) ;

            return diag.GetErrorCount() == 0;
        }

        // [2] Statement = [Expression] ';'.
        private bool IsStatement(IStatementHolder statementHolder)
        {
            IStatement statement;
            if (IsBlock(out statement)) {}
            else if (IsIfStatement(out statement)) {}
            else if (IsWhileStatement(out statement)) {}
            else if (IsTerminalExpression(out statement))
            {
                if (!CheckSpecialSymbol(";")) Error("\";\" expected to follow this expression.");
            }
            else if (IsStopStatement(out statement)) {}
            else
            {
                Error("Unknown statement.");
            }
            statementHolder.Statements.Add(statement);
            return true;
        }

        private bool IsBlock(out IStatement statement)
        {
            statement = null;
            return false;
        }

        private bool IsIfStatement(out IStatement statement)
        {
            statement = null;
            return false;
        }

        private bool IsWhileStatement(out IStatement statement)
        {
            statement = null;
            return false;
        }

        private bool IsStopStatement(out IStatement statement)
        {
            statement = null;
            return false;
        }

        private bool IsTerminalExpression(out IStatement statement)
        {
            var terminalStatement = new TerminalExpressionStatement();
            if (IsExpression(terminalStatement))
            {
                statement = terminalStatement;
                return true;
            }
            else
            {
                statement = null;
                return false;
            }
        }

        // [3] Expression = BitwiseAndExpression {'|' BitwiseAndExpression}.
        private bool IsExpression(IExpressionHolder expressionHolder)
        {
            var expression = new Expression();
            if (!IsBitwiseAndExpression(expression)) return false;
            while (CheckSpecialSymbol("|"))
            {
                IsBitwiseAndExpression(expression);
            }
            expressionHolder.Expression = expression;
            return true;
        }

        // [4] BitwiseAndExpression = AdditiveExpression {'&' AdditiveExpression}.
        private bool IsBitwiseAndExpression(Expression expression)
        {
            var bitwiseExpression = new BitwiseExpression();
            if (!IsAdditiveExpression(bitwiseExpression)) return false;
            while (CheckSpecialSymbol("&"))
            {
                IsAdditiveExpression(bitwiseExpression);
            }
            expression.Operands.Add(bitwiseExpression);
            return true;
        }

        // [5] AdditiveExpression = MultiplicativeExpression {('+' | '-') MultiplicativeExpression}.
        private bool IsAdditiveExpression(BitwiseExpression bitwiseExpression)
        {
            var additiveExpression = new AdditiveExpression();
            if (!IsMultiplicativeExpression(additiveExpression)) return false;
            while (true)
            {
                if (CheckSpecialSymbol("+"))
                {
                    additiveExpression.Operations.Add(Operation.Summation);
                }
                else if (CheckSpecialSymbol("-"))
                {
                    additiveExpression.Operations.Add(Operation.Subtraction);
                }
                else
                {
                    break;
                }
                IsMultiplicativeExpression(additiveExpression);
            }
            bitwiseExpression.Operands.Add(additiveExpression);
            return true;
        }

        // [6] MultiplicativeExpression = PrimaryExpression {('*' | '/' | '%') PrimaryExpression}.
        private bool IsMultiplicativeExpression(AdditiveExpression additiveExpression)
        {
            var multiplicativeExpression = new MultiplicativeExpression();
            if (!IsPrimaryExpression(multiplicativeExpression)) return false;
            while (true)
            {
                if (CheckSpecialSymbol("*"))
                {
                    multiplicativeExpression.Operations.Add(Operation.Multiplication);
                }
                else if (CheckSpecialSymbol("/"))
                {
                    multiplicativeExpression.Operations.Add(Operation.Division);
                }
                else if (CheckSpecialSymbol("%"))
                {
                    multiplicativeExpression.Operations.Add(Operation.Percentage);
                }
                else
                {
                    break;
                }
                IsPrimaryExpression(multiplicativeExpression);
            }
            additiveExpression.Operands.Add(multiplicativeExpression);
            return true;
        }

        // [7] PrimaryExpression = Ident ['=' Expression] | '~' PrimaryExpression | '++' Ident |
        // '--' Ident | Ident '++' | Ident '--' | Number | PrintFunc | ScanfFunc | '(' Expression ')'.
        private bool IsPrimaryExpression(MultiplicativeExpression multiplicativeExpression)
        {
            var result = true;
            var identToken = token as IdentToken;
            if (CheckIsIdent())
            {
                if (!IsVariableAssignment(identToken.value, multiplicativeExpression))
                {
                    if (CheckSpecialSymbol("++"))
                    {
                        var postIncrement = new VariablePostIncrement();
                        postIncrement.Name = identToken.value;
                        multiplicativeExpression.Operands.Add(postIncrement);
                    }
                    else if (CheckSpecialSymbol("--"))
                    {
                        var postDecrement = new VariablePostDecrement();
                        postDecrement.Name = identToken.value;
                        multiplicativeExpression.Operands.Add(postDecrement);
                    }
                    else
                    {
                        var variableIdent = new VariableIdent();
                        variableIdent.Name = identToken.value;
                        multiplicativeExpression.Operands.Add(variableIdent);
                    }
                }
            }
            else if (IsLogicalNot(multiplicativeExpression)) ;
            else if (IsPreIncrementation(multiplicativeExpression)) ;
            else if (IsPreDecrementation(multiplicativeExpression)) ;
            else if (IsNumber(multiplicativeExpression)) ;
            else if (IsPrintFunc(multiplicativeExpression)) ;
            else if (IsScanFunc(multiplicativeExpression)) ;
            else
            {
                var parenthesesExpression = new ParenthesesExpression();
                if (CheckSpecialSymbol("(") &&
                    IsExpression(parenthesesExpression) &&
                    CheckSpecialSymbol(")")
                )
                {
                    multiplicativeExpression.Operands.Add(parenthesesExpression);
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        // [8] PrintFunc = 'printf' '(' Expression ')'.
        private bool IsPrintFunc(MultiplicativeExpression multiplicativeExpression)
        {
            if (!CheckKeyword("printf")) return false;
            var printFunc = new PrintFunction();
            if (!CheckSpecialSymbol("(")) Error("Expected '(' symbol.");
            if (IsExpression(printFunc))
            {
                multiplicativeExpression.Operands.Add(printFunc);
            }
            else
            {
                return false;
            }
            if (!CheckSpecialSymbol(")")) return false;
            return true;
        }

        // [9] ScanfFunc = 'scanf' '(' ')'.
        private bool IsScanFunc(MultiplicativeExpression multiplicativeExpression)
        {
            if (!CheckKeyword("scanf")) return false;
            if (!CheckSpecialSymbol("(")) return false;
            if (!CheckSpecialSymbol(")")) return false;
            var scanFunc = new ScanFunction();
            multiplicativeExpression.Operands.Add(scanFunc);
            return true;
        }

        private bool IsVariableAssignment(string ident, MultiplicativeExpression multiplicativeExpression)
        {
            if (!CheckSpecialSymbol("=")) return false;
            var variableAssignment = new VariableAssignment();
            variableAssignment.Name = ident;
            if (!IsExpression(variableAssignment)) Error("Expected expression.");
            multiplicativeExpression.Operands.Add(variableAssignment);
            return true;
        }

        private bool IsLogicalNot(MultiplicativeExpression multiplicativeExpression)
        {
            var logicalNotExpression = new LogicalNotExpression();
            var innerMultiplicativeExpression = new MultiplicativeExpression();
            if (!CheckSpecialSymbol("~")) return false;
            if (!IsPrimaryExpression(innerMultiplicativeExpression)) return false;
            logicalNotExpression.PrimaryExpression = innerMultiplicativeExpression.Operands[0];
            multiplicativeExpression.Operands.Add(logicalNotExpression);
            return true;
        }

        private bool IsPreIncrementation(MultiplicativeExpression multiplicativeExpression)
        {
            if (!CheckSpecialSymbol("++")) return false;
            var preIncrement = new VariablePreIncrement();
            var identToken = token as IdentToken;
            if (!CheckIsIdent()) return false;
            preIncrement.Name = identToken.value;
            multiplicativeExpression.Operands.Add(preIncrement);
            return true;
        }

        private bool IsPreDecrementation(MultiplicativeExpression multiplicativeExpression)
        {
            if (!CheckSpecialSymbol("--")) return false;
            var preDecrement = new VariablePreDecrement();
            var identToken = token as IdentToken;
            if (!CheckIsIdent()) return false;
            preDecrement.Name = identToken.value;
            multiplicativeExpression.Operands.Add(preDecrement);
            return true;
        }

        private bool IsNumber(MultiplicativeExpression multiplicativeExpression)
        {
            var numberToken = token as NumberToken;
            if (numberToken != null)
            {
                var number = new Number();
                number.Value = numberToken.value;
                multiplicativeExpression.Operands.Add(number);
                ReadNextToken();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}