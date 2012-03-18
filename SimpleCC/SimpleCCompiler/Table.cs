
using System.Collections.Generic;
using SimpleC.SymbolTable;
using System.Reflection;

namespace SimpleC
{
    class Table
    {
        private Stack<Dictionary<string, TableSymbol>> symbolTable;
        private Dictionary<string, TableSymbol> universeScope;
        private Dictionary<string, TableSymbol> fieldScope;
        private List<string> references;

        public Table(List<string> references)
        {
            this.symbolTable = new Stack<Dictionary<string, TableSymbol>>();
            this.universeScope = BeginScope();
            this.fieldScope = BeginScope();
            this.references = references;
            foreach (string assemblyRef in references)
            {
                Assembly.Load(assemblyRef);
            }
        }

        public Dictionary<string, TableSymbol> BeginScope()
        {
            symbolTable.Push(new Dictionary<string, TableSymbol>());
            return symbolTable.Peek();
        }

        public TableSymbol AddToUniverse(TableSymbol symbol)
        {
            universeScope.Add(symbol.value, symbol);
            return symbol;
        }
    }
}
