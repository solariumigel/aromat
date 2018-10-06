using System.Collections.Generic;
using System.Linq;

namespace Aromat.CodeAnalysis
{
    class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IEnumerable<string> Diagnostics { get; }

        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);

            var tree =  parser.Parse();

            return tree;
        }
    }
}