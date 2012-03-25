using SimpleC.SymbolTable;
using SimpleC.Tokens;
using System;

namespace SimpleC
{
    class Parser
    {
        private Scanner scanner;
        private Emitter emit;
        private Table symbolTable;
        private Diagnostics diag;
        private Token token;

        public Parser(Scanner scanner, Emitter emit, Table symbolTable, Diagnostics diag)
        {
            this.scanner = scanner;
            this.emit = emit;
            this.symbolTable = symbolTable;
            this.diag = diag;
        }

        public bool Parse()
        {
            ReadNextToken();
            return IsProgram() && token is EOFToken;
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

        /* Grammar
         */

        // [1] Program = {Statement}
        public bool IsProgram()
        {
            while (IsStatement());

            return diag.GetErrorCount() == 0;
        }

        // [2] Statement = [Expression] ';'.
        private bool IsStatement()
        {
            if (!IsExpression()) Error("Expression is not well formed.");
            if (!CheckSpecialSymbol(";")) Error("\";\" expected.");
            return true;
        }

        // [3] Expression = BitwiseAndExpression {'|' BitwiseAndExpression}.
        private bool IsExpression()
        {
            if (!IsBitwiseAndExpression()) return false;
            while (CheckSpecialSymbol("|"))
            {
                IsBitwiseAndExpression();
            }
            return true;
        }

        // [4] BitwiseAndExpression = AdditiveExpression {'&' AdditiveExpression}.
        private bool IsBitwiseAndExpression()
        {
            if (!IsAdditiveExpression()) return false;
            while (CheckSpecialSymbol("&"))
            {
                IsAdditiveExpression();
            }
            return true;
        }

        // [5] AdditiveExpression = MultiplicativeExpression {('+' | '-') MultiplicativeExpression}.
        private bool IsAdditiveExpression()
        {
            if (!IsMultiplicativeExpression()) return false;
            while (CheckSpecialSymbol("+") || CheckSpecialSymbol("-"))
            {
                IsMultiplicativeExpression();
            }
            return true;
        }

        // [6] MultiplicativeExpression = PrimaryExpression {('*' | '/' | '%') PrimaryExpression}.
        private bool IsMultiplicativeExpression()
        {
            if (!IsPrimaryExpression()) return false;
            while (CheckSpecialSymbol("*") || CheckSpecialSymbol("/") || CheckSpecialSymbol("%"))
            {
                IsPrimaryExpression();
            }
            return true;
        }

        // [7] PrimaryExpression = Ident ['=' Expression] | '~' PrimaryExpression | '++' Ident | '--' Ident | Ident '++' | Ident '--' | 
        //                Number | PrintFunc | ScanfFunc | '(' Expression ')'.
        private bool IsPrimaryExpression()
        {
            var result = true;
            if (IsVariableAssignment()) ;
            else if (IsLogicalNot()) ;
            else if (IsPreIncrementation()) ;
            else if (IsPreDecrementation()) ;
            else if (IsPostIncrementation()) ;
            else if (IsPostDecrementation()) ;
            else if (IsNumber()) ;
            else if (IsPrintFunc()) ;
            else if (IsScanFunc()) ;
            else if (IsLogicalNot()) ;
            else
            {
                if (!(
                    CheckSpecialSymbol("(") &&
                    IsExpression() &&
                    CheckSpecialSymbol(")")
                ))
                {
                    result = false;
                }
            }

            return result;
        }

        // [8] PrintFunc = 'printf' '(' Expression ')'.
        private bool IsPrintFunc()
        {
            if (!CheckKeyword("printf")) return false;
            if (!CheckSpecialSymbol("(")) return false;
            if (!IsExpression()) return false;
            if (!CheckSpecialSymbol(")")) return false;
            return true;
        }

        // [9] ScanfFunc = 'scanf' '(' ')'.
        private bool IsScanFunc()
        {
            if (!CheckKeyword("scanf")) return false;
            if (!CheckSpecialSymbol("(")) return false;
            if (!CheckSpecialSymbol(")")) return false;
            return true;
        }

        private bool IsVariableAssignment()
        {
            if (!CheckIsIdent()) return false;
            if (CheckSpecialSymbol("="))
            {
                if (!IsExpression()) return false;
            }
            return true;
        }

        private bool IsLogicalNot()
        {
            if (!CheckSpecialSymbol("~")) return false;
            if (!IsPrimaryExpression()) return false;
            return true;
        }

        private bool IsPreIncrementation()
        {
            if (!CheckSpecialSymbol("++")) return false;
            if (!CheckIsIdent()) return false;
            return true;
        }

        private bool IsPreDecrementation()
        {
            if (!CheckSpecialSymbol("--")) return false;
            if (!CheckIsIdent()) return false;
            return true;
        }

        private bool IsPostIncrementation()
        {
            if (!CheckIsIdent()) return false;
            if (!CheckSpecialSymbol("++")) return false;
            return true;
        }

        private bool IsPostDecrementation()
        {
            if (!CheckIsIdent()) return false;
            if (!CheckSpecialSymbol("--")) return false;
            return true;
        }

        private bool IsNumber()
        {
            bool result = (token is NumberToken);
            if (result) ReadNextToken();
            return result;
        }
    }
}