using System.Collections.Generic;

namespace Aromat.CodeAnalysis
{
    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        
        private List<string> _dianostics = new List<string>();
        private int _position;

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer =  new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();

                if(token.Kind != SyntaxKind.WhiteSpaceToken && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            }
            while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _dianostics.AddRange(lexer.Dianostics);
        }

        public IEnumerable<string> Diangostics => _dianostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if(index >= _tokens.Length)
            {
                return _tokens[_tokens.Length -1];
            }
            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if(Current.Kind == kind)
            {
                return NextToken();
            }

            _dianostics.Add($"ERROR: Unexpected token: <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        public SyntaxTree Parse()
        {
            var expression =  ParseTerm();

            var endOfFileToken = Match(SyntaxKind.EndOfFileToken); 

            return new SyntaxTree(_dianostics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesisToken);

                return new PerenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);

        }
    }
}