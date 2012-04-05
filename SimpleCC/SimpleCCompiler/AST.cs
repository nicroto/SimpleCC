using System.Collections.Generic;
using System;
namespace SimpleC
{
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

    public enum Operation
    {
        Summation,
        Subtraction,
        Multiplication,
        Division,
        Percentage,
        Or,
        And,
        Not
    }

    public enum InDeCrementStatus
    {
        PreIncrement,
        PreDecrement,
        PostIncrement,
        PostDecrement
    }

    public class AdditiveExpression
    {
        public List<MultiplicativeExpression> Operands = new List<MultiplicativeExpression>();
        public List<Operation> Operations = new List<Operation>();
    }

    public class MultiplicativeExpression : IAdditiveOperand
    {
        public List<PrimaryExpression> Operands = new List<PrimaryExpression>();
        public List<Operation> Operations = new List<Operation>();
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
