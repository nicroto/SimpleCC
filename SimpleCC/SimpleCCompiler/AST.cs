using System.Collections.Generic;
using System;
namespace SimpleC
{
    #region Coppied AST's
    /* <stmt> := <ident> = <expr>
        | read_int <ident>
        | print <expr>
      */
    public abstract class Stmt
    {
    }

    // print <expr>
    public class Print : Stmt
    {
        public Expr Expr;
    }

    // <ident> = <expr>
    public class Assign : Stmt
    {
        public string Ident;
        public Expr Expr;
    }

    // read_int <ident>
    public class ReadInt : Stmt
    {
        public string Ident;
    }

    public abstract class Expr
    {
    }

    // <int> := <digit>+
    public class IntLiteral : Expr
    {
        public int Value;
    }

    // <ident> := <char> <ident_rest>*
    // <ident_rest> := <char> | <digit>
    public class Variable : Expr
    {
        public string Ident;
    }

    // <bin_expr> := <expr> <bin_op> <expr>
    public class BinExpr : Expr
    {
        public Expr Left;
        public Expr Right;
        public BinOp Op;
    }

    // <bin_op> := + | - | * | /
    public enum BinOp
    {
        Add,
        Sub,
        Mul,
        Div
    }
    #endregion

    public interface IExpressionHolder
    {
        Expression Expression { get; set; }
    }

    public interface IAdditiveOperand
    {
    }

    public class Statement : IExpressionHolder
    {
        public Expression Expression { get; set; }
    }

    public class Expression
    {
        public List<BitwiseExpression> Operands = new List<BitwiseExpression>();
    }

    public class BitwiseExpression
    {
        public List<AdditiveExpression> Operands = new List<AdditiveExpression>();
    }

    public enum AdditiveOperations
    {
        Summation,
        Subtraction
    }

    public class AdditiveExpression
    {
        public List<MultiplicativeExpression> Operands = new List<MultiplicativeExpression>();
        public List<AdditiveOperations> Operations = new List<AdditiveOperations>();
    }

    public enum MultiplicativeOperations
    {
        Multiplication,
        Division,
        Percentage
    }

    public class MultiplicativeExpression : IAdditiveOperand
    {
        public List<PrimaryExpression> Operands = new List<PrimaryExpression>();
        public List<MultiplicativeOperations> Operations = new List<MultiplicativeOperations>();
    }

    public interface PrimaryExpression
    {
    }

    public class VariableIdent : PrimaryExpression
    {
        public string Name = String.Empty;
    }

    public class VariableAssignment : PrimaryExpression, IExpressionHolder
    {
        public string Name = String.Empty;
        public Expression Expression { get; set; }
    }

    public class VariablePostIncrement : PrimaryExpression
    {
        public string Name = String.Empty;
    }

    public class VariablePostDecrement : PrimaryExpression
    {
        public string Name = String.Empty;
    }

    public class LogicalNotExpression : PrimaryExpression
    {
        public PrimaryExpression PrimaryExpression;
    }

    public class VariablePreIncrement : PrimaryExpression
    {
        public string Name = String.Empty;
    }

    public class VariablePreDecrement : PrimaryExpression
    {
        public string Name = String.Empty;
    }

    public class Number : PrimaryExpression
    {
        public int Value;
    }

    public class PrintFunction : PrimaryExpression, IExpressionHolder
    {
        public Expression Expression { get; set; }
    }

    public class ScanFunction : PrimaryExpression
    {
    }

    public class ParenthesesExpression : PrimaryExpression, IExpressionHolder
    {
        public Expression Expression { get; set; }
    }
}
