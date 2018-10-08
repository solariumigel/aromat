namespace Aromat.CodeAnalysis
{

    class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public decimal Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private decimal EvaluateExpression(ExpressionSyntax root)
        {
            return root.Calculate();
        }
    }
}