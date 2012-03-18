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
        private Tokens.Token token;

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
            AddPredefinedSymbols();
            return IsProgram() && token is EOFToken;
        }

        public void ReadNextToken()
        {
            token = scanner.Next();
        }

        public void AddPredefinedSymbols()
        {
            symbolTable.AddToUniverse(new PrimitiveTypeSymbol(new IdentToken(-1, -1, "int"), typeof(System.Int32)));
            symbolTable.AddToUniverse(new PrimitiveTypeSymbol(new IdentToken(-1, -1, "bool"), typeof(System.Boolean)));
            symbolTable.AddToUniverse(new PrimitiveTypeSymbol(new IdentToken(-1, -1, "double"), typeof(System.Double)));
            symbolTable.AddToUniverse(new PrimitiveTypeSymbol(new IdentToken(-1, -1, "char"), typeof(System.Char)));
            symbolTable.AddToUniverse(new PrimitiveTypeSymbol(new IdentToken(-1, -1, "string"), typeof(System.String)));
        }

        // Grammar
        public bool IsProgram()
        {
            throw new NotImplementedException();
        }
    }
}
