using System;
using System.Collections.Generic;
using System.IO;
using SimpleC;

class Program
{
    static int Main(string[] args)
    {
        args = new string[] { @"C:\SourceCodeRepos\SimpleCC\SimpleCC\Console\bin\Debug\test.scs" };
        if (args.Length == 0)
        {
            Console.WriteLine("Syntax: scsc {/r:filename} <source file> [<result exe file>]");
            return -1;
        }

        int i = 0;
        List<string> references = new List<string>();
        references.Add("mscorlib");
        while (args[i].StartsWith("/r:"))
        {
            references.Add(args[i].Substring(3));
            i++;
        }

        string assemblyName;
        if (args.Length == i + 2) assemblyName = args[i + 1];
        else assemblyName = Path.ChangeExtension(args[i], "exe");

        Compiler.AddReferences(references);
        Compiler.Compile(args[i], assemblyName);

        return 0;
    }
}