using System.IO;
using SimpleC.Tokens;
using System;

namespace SimpleC
{
    class Scanner
    {
        const char EOF = '\u001a';
        const char CR = '\r';
        const char LF = '\n';

        private TextReader reader;
        private int line;
        private int column;
        private char ch;

        public Scanner(TextReader reader)
        {
            this.reader = reader;
            this.line = 1;
            this.column = 0;
            ReadNextChar();
        }

        public void ReadNextChar()
        {
            int ch1 = reader.Read();
            column++;
            ch = (ch1 < 0) ? EOF : (char)ch1;
            if (ch == CR)
            {
                line++;
                column = 0;
            }
            else if (ch == LF)
            {
                column = 0;
            }
        }

        public Token Next()
        {
            throw new NotImplementedException();
        }
    }
}
