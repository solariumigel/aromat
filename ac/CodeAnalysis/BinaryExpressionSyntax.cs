using System;
using System.Collections.Generic;

namespace Aromat.CodeAnalysis
{
    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public override decimal Calculate()
        {
            if(OperatorToken.Kind == SyntaxKind.PlusToken)
            {
                return Left.Calculate() + Right.Calculate();
            }
            if(OperatorToken.Kind == SyntaxKind.MinusToken)
            {
                return Left.Calculate() - Right.Calculate();
            }
            if(OperatorToken.Kind == SyntaxKind.StarToken)
            {
                return Left.Calculate() * Right.Calculate();
            }
            if(OperatorToken.Kind == SyntaxKind.SlashToken)
            {
                return Left.Calculate() / Right.Calculate();
            }

            throw new Exception($"Unexpected binary operator {OperatorToken.Kind}");
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}