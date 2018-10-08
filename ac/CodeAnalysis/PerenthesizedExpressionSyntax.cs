using System.Collections.Generic;

namespace Aromat.CodeAnalysis
{
    sealed class PerenthesizedExpressionSyntax : ExpressionSyntax
    {

        public PerenthesizedExpressionSyntax(SyntaxNode openParenthesisToken, ExpressionSyntax expressionSyntax, SyntaxNode CloseParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            ExpressionSyntax = expressionSyntax;
            this.CloseParenthesisToken = CloseParenthesisToken;
        }
        public override SyntaxKind Kind => SyntaxKind.ParenthesisToken;

        public SyntaxNode OpenParenthesisToken { get; }
        public ExpressionSyntax ExpressionSyntax { get; }
        public SyntaxNode CloseParenthesisToken { get; }

        public override decimal Calculate()
        {
            return ExpressionSyntax.Calculate();
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return ExpressionSyntax;
            yield return CloseParenthesisToken;
        }
    }
}