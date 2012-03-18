namespace SimpleC.Tokens
{
    abstract class LiteralToken: Token
    {
        public LiteralToken(int line, int column) : base(line, column) { }
    }
}
