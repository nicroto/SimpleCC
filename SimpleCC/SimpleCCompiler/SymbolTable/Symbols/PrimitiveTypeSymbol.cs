using System;
using SimpleC.Tokens;

namespace SimpleC.SymbolTable
{
    class PrimitiveTypeSymbol: TypeSymbol
    {
        public PrimitiveTypeSymbol(IdentToken token, Type type) : base(token, type) { }
    }
}
