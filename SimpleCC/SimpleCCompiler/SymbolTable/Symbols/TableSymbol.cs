using SimpleC.Tokens;

namespace SimpleC.SymbolTable
{
    abstract class TableSymbol: IdentToken
    {
        public TableSymbol(int line, int column, string value) : base(line, column, value) { }
    }
}
