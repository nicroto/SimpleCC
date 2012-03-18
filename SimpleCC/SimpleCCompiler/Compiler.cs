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
            TextReader reader = new StreamReader(file);
            Scanner scanner = new Scanner(reader);
            Table symbolTable = new Table(references);
            Emitter emit = new Emitter(assemblyName, symbolTable);
            Parser parser = new Parser(scanner, emit, symbolTable, diag);

            diag.BeginSourceFile(file);
            bool isProgram = parser.Parse();
            diag.EndSourceFile();

            if (isProgram)
            {
                emit.WriteExecutable();
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
