using System.Text;

namespace SimpleC.Tokens
{
    class NumberToken: LiteralToken
    {
        public int value;

        public NumberToken(int line, int column, int value)
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
