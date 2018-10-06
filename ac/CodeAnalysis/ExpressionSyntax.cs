namespace Aromat.CodeAnalysis
{
    abstract class ExpressionSyntax : SyntaxNode
    {
        public abstract int Calculate();
    }
}