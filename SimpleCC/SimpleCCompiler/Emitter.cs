using System.Reflection;
using System.IO;
using System.Reflection.Emit;
using System.Collections.Generic;
using System;

namespace SimpleC
{
    class Emitter
    {
        ILGenerator il = null;
        Dictionary<string, LocalBuilder> symbolTable;

        public Emitter(List<Statement> statements, string moduleName)
        {
            if (Path.GetFileName(moduleName) != moduleName)
            {
                throw new System.Exception("can only output into current directory!");
            }

            AssemblyName name = new AssemblyName(Path.GetFileNameWithoutExtension(moduleName));
            AssemblyBuilder asmb = System.AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            ModuleBuilder modb = asmb.DefineDynamicModule(moduleName);
            TypeBuilder typeBuilder = modb.DefineType("Foo");

            MethodBuilder methb = typeBuilder.DefineMethod("Main", MethodAttributes.Static, typeof(void), System.Type.EmptyTypes);

            // CodeGenerator
            this.il = methb.GetILGenerator();
            this.symbolTable = new Dictionary<string, LocalBuilder>();

            // Go Compile!
            foreach(var statement in statements)
                this.GenerateStatement(statement);

            il.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            modb.CreateGlobalFunctions();
            asmb.SetEntryPoint(methb);
            asmb.Save(moduleName);
            this.symbolTable = null;
            this.il = null;
        }

        private void GenerateStatement(Statement statement)
        {
            GenerateExpression(statement.Expression);
        }

        private void GenerateExpression(Expression expression)
        {
            var bitwiseExpressions = expression.Operands;
            if (bitwiseExpressions.Count > 0)
            {
                GenerateBitwiseExpression(bitwiseExpressions[0]);
                if (bitwiseExpressions.Count > 1)
                    for (var i = 1; i < bitwiseExpressions.Count; i++)
                    {
                        // TODO: Emit '|'
                        GenerateBitwiseExpression(bitwiseExpressions[i]);
                    }
            }
        }

        private void GenerateBitwiseExpression(BitwiseExpression bitwiseExpression)
        {
            var additiveExpressions = bitwiseExpression.Operands;
            if (additiveExpressions.Count > 0)
            {
                GenerateAdditiveExpression(additiveExpressions[0]);
                if (additiveExpressions.Count > 1)
                    for (var i = 1; i < additiveExpressions.Count; i++)
                    {
                        // TODO: Emit '&'
                        GenerateAdditiveExpression(additiveExpressions[i]);
                    }
            }
        }

        private void GenerateAdditiveExpression(AdditiveExpression additiveExpression)
        {
            var multiplicativeExpressions = additiveExpression.Operands;
            var operations = additiveExpression.Operations;
            if (multiplicativeExpressions.Count > 0)
            {
                GenerateMultiplicativeExpression(multiplicativeExpressions[0]);
                if (multiplicativeExpressions.Count > 1)
                    for (var i = 1; i < multiplicativeExpressions.Count; i++)
                    {
                        // TODO: Emit operations[i-1]
                        GenerateMultiplicativeExpression(multiplicativeExpressions[i]);
                    }
            }
        }

        private void GenerateMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression)
        {
            var primaryExpressions = multiplicativeExpression.Operands;
            var operations = multiplicativeExpression.Operations;
            if (primaryExpressions.Count > 0)
            {
                GeneratePrimaryExpression(primaryExpressions[0]);
                if (primaryExpressions.Count > 1)
                    for (var i = 1; i < primaryExpressions.Count; i++)
                    {
                        // TODO: Emit operations[i-1]
                        GeneratePrimaryExpression(primaryExpressions[i]);
                    }
            }
        }

        private void GeneratePrimaryExpression(PrimaryExpression primaryExpression)
        {
            if (primaryExpression is VariableIdent)
            {

            }
            else if (primaryExpression is VariableAssignment)
            {
                var variableAssignment = primaryExpression as VariableAssignment;
                var name = variableAssignment.Name;
                if (!this.symbolTable.ContainsKey(name))
                    this.symbolTable[name] = this.il.DeclareLocal(typeof(int));
            }
            else if (primaryExpression is VariablePostIncrement)
            {
            }
            else if (primaryExpression is VariablePostDecrement)
            {
            }
            else if (primaryExpression is LogicalNotExpression)
            {
            }
            else if (primaryExpression is VariablePreIncrement)
            {
            }
            else if (primaryExpression is VariablePreDecrement)
            {
            }
            else if (primaryExpression is Number)
            {
            }
            else if (primaryExpression is PrintFunction)
            {
            }
            else if (primaryExpression is ScanFunction)
            {
            }
            else if (primaryExpression is ParenthesesExpression)
            {
            }
        }

        private void Store(string name)
        {
            this.il.Emit(OpCodes.Stloc, this.symbolTable[name]);
        }

        private void GenExpr(Expr expr)
        {
            if (expr is IntLiteral)
            {
                this.il.Emit(OpCodes.Ldc_I4, ((IntLiteral)expr).Value);
            }
            else if (expr is Variable)
            {
                string ident = ((Variable)expr).Ident;

                if (!this.symbolTable.ContainsKey(ident))
                {
                    throw new Exception("undeclared variable '" + ident + "'");
                }

                this.il.Emit(OpCodes.Ldloc, this.symbolTable[ident]);
            }
            else
            {
                throw new Exception("don't know how to generate " + expr.GetType().Name);
            }
        }
    }
}