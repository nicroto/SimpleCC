using System.Collections.Generic;
using System.IO;

namespace SimpleC
{
    public static class Compiler
    {
        private static List<string> references = new List<string>();

        public static bool Compile(string file, string assemblyName)
        {
            return Compiler.Compile(file, assemblyName, new DefaultDiagnostics { });
        }

        public static bool Compile(string file, string assemblyName, Diagnostics diag)
        {
            var reader = new StreamReader(file);
            var scanner = new Scanner(reader);
            var parser = new Parser(scanner, diag);

            diag.BeginSourceFile(file);
            bool isProgram = parser.Parse();
            diag.EndSourceFile();

            if (isProgram)
            {
                var emit = new Emitter(parser.Result, assemblyName);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void AddReferences(List<string> references)
        {
            Compiler.references.AddRange(references);
        }
    }
}
