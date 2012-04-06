using System;
using System.IO;
using SimpleC;

class Program
{
    static int Main(string[] args)
    {
        args = args.Length > 0 ?
            args : new string[] { Path.Combine(Directory.GetCurrentDirectory(), "test.scs") };
        if (args.Length == 0)
        {
            Console.WriteLine("Syntax: console <source file>");
            return -1;
        }

        var assemblyName = Path.GetFileNameWithoutExtension(args[0]) + ".exe";

        Compiler.Compile(args[0], assemblyName);

        return 0;
    }
}