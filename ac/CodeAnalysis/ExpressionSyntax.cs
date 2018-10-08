namespace Aromat.CodeAnalysis
{
    abstract class ExpressionSyntax : SyntaxNode
    {
        public abstract decimal Calculate();
    }
}