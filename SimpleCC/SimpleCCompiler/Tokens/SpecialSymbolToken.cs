﻿using System.Text;

namespace SimpleC.Tokens
{
    class SpecialSymbolToken: Token
    {
        public string value;

        public SpecialSymbolToken(int line, int column, string value)
            : base(line, column)
        {
            this.value = value;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendFormat("line {0}, column {1}: {2} - {3}", line, column, value, GetType());
            return s.ToString();
        }
    }
}
