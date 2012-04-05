using System;
using System.IO;
using System.Text;
using SimpleC.Tokens;

namespace SimpleC
{
    class Scanner
    {
        static readonly string keywords =
            " scanf printf ";
        static readonly string specialSymbolsA =
            "%()/*;~";
        static readonly string specialSymbolsB =
            "&|+-=";
        static readonly string specialSymbolPairs =
            " ++ -- ";

        const char EOF = '\u001a';
        const char CR = '\r';
        const char LF = '\n';
        const char Escape = '\\';

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

        public char UnEscape(char c)
        {
            switch (c)
            {
                case 't': return '\t';
                case 'n': return '\n';
                case 'r': return '\r';
                case 'f': return '\f';
                case '\'': return '\'';
                case '"': return '\"';
                case '0': return '\0';
                case Escape: return Escape;
                default: return c;
            }
        }

        public Token Next()
        {
            int start_column;
            int start_line;
            while (true)
            {
                start_column = column;
                start_line = line;
                if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch == '.')
                {
                    return ScanIdentOrKeyword(start_line, start_column);
                }
                else if (ch >= '0' && ch <= '9')
                {
                    return ScanNumber(start_line, start_column);
                }
                else if (specialSymbolsA.Contains(ch.ToString()))
                {
                    return ScanSpecialSymbolA(start_line, start_column);
                }
                else if (specialSymbolsB.Contains(ch.ToString()))
                {
                    return ScanSpecialSymbolPairsOrSpecialSymbolB(start_line, start_column);
                }
                else if (ch == ' ' || ch == '\t' || ch == CR || ch == LF)
                {
                    ReadNextChar();
                    continue;
                }
                else if (ch == EOF)
                {
                    return new EOFToken(start_line, start_column);
                }
                else
                {
                    return ScanUnrecognizedToken(start_line, start_column);
                }
            }
        }

        private Token ScanIdentOrKeyword(int start_line, int start_column)
        {
            StringBuilder s = new StringBuilder();
            while (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch == '.' || ch >= '0' && ch <= '9')
            {
                s.Append(ch);
                ReadNextChar();
            }
            string id = s.ToString();
            if (keywords.Contains(" " + id + " "))
            {
                return new KeywordToken(start_line, start_column, id);
            }
            return new IdentToken(start_line, start_column, id);
        }

        private Token ScanNumber(int start_line, int start_column)
        {
            StringBuilder s = new StringBuilder();
            while (ch >= '0' && ch <= '9')
            {
                s.Append(ch);
                ReadNextChar();
            }
            return new NumberToken(start_line, start_column, Convert.ToInt32(s.ToString()));
        }

        private Token ScanSpecialSymbolA(int start_line, int start_column)
        {
            char ch1 = ch;
            ReadNextChar();
            return new SpecialSymbolToken(start_line, start_column, ch1.ToString());
        }

        private Token ScanSpecialSymbolPairsOrSpecialSymbolB(int start_line, int start_column)
        {
            char ch1 = ch;
            ReadNextChar();
            char ch2 = ch;
            if (specialSymbolPairs.Contains(" " + ch1 + ch2 + " "))
            {
                ReadNextChar();
                return new SpecialSymbolToken(start_line, start_column, ch1.ToString() + ch2);
            }
            return new SpecialSymbolToken(start_line, start_column, ch1.ToString());
        }

        private Token ScanUnrecognizedToken(int start_line, int start_column)
        {
            string s = ch.ToString();
            ReadNextChar();
            return new OtherToken(start_line, start_column, s);
        }
    }
}
