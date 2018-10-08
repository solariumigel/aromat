using System.Collections.Generic;

namespace Aromat.CodeAnalysis
{
    class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken; 
        }

        public override decimal Calculate()
        {
            return (decimal)NumberToken.Value;
        }
    }
}