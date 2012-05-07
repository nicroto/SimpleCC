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

        public Emitter(List<IStatement> statements, string moduleName)
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

            this.il.Emit(OpCodes.Ldstr, "The Program has finished execution. Press any key to exit.");
            this.il.Emit(OpCodes.Call, typeof(Console).GetMethod(
                "WriteLine",
                new Type[] { typeof(string) }
            ));
            this.il.Emit(OpCodes.Call, typeof(System.Console).GetMethod(
                "ReadKey",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new System.Type[] { },
                null
            ));

            il.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            modb.CreateGlobalFunctions();
            asmb.SetEntryPoint(methb);
            asmb.Save(moduleName);
            this.symbolTable = null;
            this.il = null;
        }

        private void GenerateStatement(IStatement statement)
        {
            switch (statement.GetType().ToString())
            {
                case "Block":
                    GenerateBlock((Block)statement);
                    break;
                case "IfStatement":
                    GenerateIfStatement((IfStatement)statement);
                    break;
                case "WhileStatement":
                    GenerateWhileStatement((WhileStatement)statement);
                    break;
                case "StopStatement":
                    GenerateStopStatement((StopStatement)statement);
                    break;
                case "TerminalExpressionStatement":
                    GenerateExpression(((TerminalExpressionStatement)statement).Expression);
                    break;
            }
        }

        private void GenerateBlock(Block block)
        {
            throw new NotImplementedException();
        }

        private void GenerateIfStatement(IfStatement ifStatement)
        {
            throw new NotImplementedException();
        }

        private void GenerateWhileStatement(WhileStatement whileStatement)
        {
            throw new NotImplementedException();
        }

        private void GenerateStopStatement(StopStatement stopStatement)
        {
            throw new NotImplementedException();
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
                        GenerateBitwiseExpression(bitwiseExpressions[i]);
                        GenerateOperation(Operation.Or);
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
                        GenerateAdditiveExpression(additiveExpressions[i]);
                        GenerateOperation(Operation.And);
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
                        GenerateMultiplicativeExpression(multiplicativeExpressions[i]);
                        GenerateOperation(operations[i-1]);
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
                        GeneratePrimaryExpression(primaryExpressions[i]);
                        GenerateOperation(operations[i - 1]);
                    }
            }
        }

        private void GeneratePrimaryExpression(PrimaryExpression primaryExpression)
        {
            if (primaryExpression is VariableIdent)
            {
                var variableIdent = primaryExpression as VariableIdent;
                var variableBuilder = this.symbolTable[variableIdent.Name];
                il.Emit(OpCodes.Ldloc, variableBuilder);
            }
            else if (primaryExpression is VariableAssignment)
            {
                var variableAssignment = primaryExpression as VariableAssignment;
                var name = variableAssignment.Name;
                if (!this.symbolTable.ContainsKey(name))
                    this.symbolTable[name] = this.il.DeclareLocal(typeof(int));
                GenerateExpression(variableAssignment.Expression);
                il.Emit(OpCodes.Stloc, this.symbolTable[name]);
            }
            else if (primaryExpression is VariablePostIncrement)
            {
                var postIncrement = primaryExpression as VariablePostIncrement;
                var variableBuilder = this.symbolTable[postIncrement.Name];
                il.Emit(OpCodes.Ldloc, variableBuilder);
                GenerateIncrementOrDecrement(variableBuilder, InDeCrementStatus.PostIncrement);
            }
            else if (primaryExpression is VariablePostDecrement)
            {
                var postDecrement = primaryExpression as VariablePostDecrement;
                var variableBuilder = this.symbolTable[postDecrement.Name];
                il.Emit(OpCodes.Ldloc, variableBuilder);
                GenerateIncrementOrDecrement(variableBuilder, InDeCrementStatus.PostDecrement);
            }
            else if (primaryExpression is VariablePreIncrement)
            {
                var preIncrement = primaryExpression as VariablePreIncrement;
                var variableBuilder = this.symbolTable[preIncrement.Name];
                il.Emit(OpCodes.Ldloc, variableBuilder);
                GenerateIncrementOrDecrement(variableBuilder, InDeCrementStatus.PreIncrement);
            }
            else if (primaryExpression is VariablePreDecrement)
            {
                var preDecrement = primaryExpression as VariablePreDecrement;
                var variableBuilder = this.symbolTable[preDecrement.Name];
                il.Emit(OpCodes.Ldloc, variableBuilder);
                GenerateIncrementOrDecrement(variableBuilder, InDeCrementStatus.PreDecrement);
            }
            else if (primaryExpression is LogicalNotExpression)
            {
                var logicalNotExpression = primaryExpression as LogicalNotExpression;
                GeneratePrimaryExpression(logicalNotExpression.PrimaryExpression);
                GenerateOperation(Operation.Not);
            }
            else if (primaryExpression is Number)
            {
                var number = primaryExpression as Number;
                il.Emit(OpCodes.Ldc_I4, number.Value);
            }
            else if (primaryExpression is PrintFunction)
            {
                var printFunction = primaryExpression as PrintFunction;
                GenerateExpression(printFunction.Expression);
                this.il.Emit(OpCodes.Call, typeof(Console).GetMethod(
                    "WriteLine",
                    new Type[] { typeof(int) }
                ));
            }
            else if (primaryExpression is ScanFunction)
            {
                this.il.Emit(OpCodes.Call, typeof(System.Console).GetMethod(
                    "ReadLine",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new System.Type[] { },
                    null
                ));
                this.il.Emit(OpCodes.Call, typeof(int).GetMethod(
                    "Parse",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(string) },
                    null
                ));
            }
            else if (primaryExpression is ParenthesesExpression)
            {
                var parenthesesExpression = primaryExpression as ParenthesesExpression;
                GenerateExpression(parenthesesExpression.Expression);
            }
        }

        private void GenerateOperation(Operation operation)
        {
            switch (operation)
            {
                case Operation.Multiplication:
                    il.Emit(OpCodes.Mul);
                    break;
                case Operation.Division:
                    il.Emit(OpCodes.Div);
                    break;
                case Operation.Percentage:
                    il.Emit(OpCodes.Rem);
                    break;
                case Operation.Summation:
                    il.Emit(OpCodes.Add);
                    break;
                case Operation.Subtraction:
                    il.Emit(OpCodes.Sub);
                    break;
                case Operation.Or:
                    il.Emit(OpCodes.Or);
                    break;
                case Operation.And:
                    il.Emit(OpCodes.And);
                    break;
                case Operation.Not:
                    il.Emit(OpCodes.Not);
                    break;
            }
        }

        private void GenerateIncrementOrDecrement(LocalBuilder variableBuilder, InDeCrementStatus status)
        {
            switch (status)
            {
                case InDeCrementStatus.PostIncrement:
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Add);
                    il.Emit(OpCodes.Stloc, variableBuilder);
                    break;
                case InDeCrementStatus.PostDecrement:
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Sub);
                    il.Emit(OpCodes.Stloc, variableBuilder);
                    break;
                case InDeCrementStatus.PreIncrement:
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Add);
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Stloc, variableBuilder);
                    break;
                case InDeCrementStatus.PreDecrement:
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Sub);
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Stloc, variableBuilder);
                    break;
            }
        }
    }
}