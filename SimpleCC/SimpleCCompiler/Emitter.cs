using System.Reflection;
using System.IO;
using System;
using System.Reflection.Emit;

namespace SimpleC
{
    class Emitter
    {
        private Table symbolTable;
        private string executableName;
        private AssemblyBuilder assembly;
        private ModuleBuilder module;
        private bool haveMainMethod;

        public Emitter(string name, Table symbolTable)
        {
            this.symbolTable = symbolTable;
            this.executableName = name;
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = Path.GetFileNameWithoutExtension(name);
            string dir = Path.GetDirectoryName(name);

            string moduleName = Path.GetFileName(name);

            this.assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save, dir);
            this.module = assembly.DefineDynamicModule(assemblyName + "Module", moduleName);
            this.haveMainMethod = false;
        }

        public void WriteExecutable()
        {
            throw new NotImplementedException();
        }
    }
}
