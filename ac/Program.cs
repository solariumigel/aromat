using System;
using System.Collections.Generic;
using System.Linq;

namespace ac
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                var parser = new Parser(line);
                var syntax = parser.Parse();

                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.WriteLine("Expressiontree: ");
                PrettyPrint(syntax.Root);
                
                Console.ForegroundColor = color;

                if(syntax.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach(var dianostics in parser.Diangostics)
                    {
                        Console.WriteLine(dianostics);
                    }
                    Console.ForegroundColor = color;
                }
                else
                {
                    var evaluator = new Evaluator(syntax.Root);

                    var result = evaluator.Evaluate();

                    Console.WriteLine($"RESULT: {result}");
                }
            }            
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "|   ";


            var lastChild = node.GetChildren().LastOrDefault();

            foreach(var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }
    }

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

        public SyntaxTree Parse()
        {
            var expression =  ParseExpression();

            var endOfFileToken = Match(SyntaxKind.EndOfFileToken); 

            return new SyntaxTree(_dianostics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken 
                || Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);

        }
    }

    class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind {get;}

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {
        public abstract int Calculate();
    }

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

        public override int Calculate()
        {
            return (int)NumberToken.Value;
        }
    }

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

        public override int Calculate()
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

    enum SyntaxKind
    {
        NumberToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        NumberExpression,
        BinaryExpression
    }

    class SyntaxToken : SyntaxNode
    {

        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public string Text{get;}

        public object Value { get;}

        public int Position { get;  }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

    class Lexer
    {
        private readonly string _text;
        private int _position;

        private List<string> _dianostics = new List<string>();
        public Lexer(string text)
        {
            _text = text;
        }

        public List<string> Dianostics{get {return _dianostics;}}

        private char Current
        {
            get
            {
                if(_position >= _text.Length)
                {
                    return '\0';
                }
                return _text[_position];
            }
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
        {

            if(_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            if(char.IsDigit(Current))
            {
                var start = _position;

                while(char.IsDigit(Current))
                {
                    Next();
                }

                var length = _position -start;
                var text = _text.Substring(start, length);

                if(!int.TryParse(text, out var number))
                {
                    _dianostics.Add($"ERROR {text} is not a number");
                }

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, number);
            }

            if(char.IsWhiteSpace(Current))
            {
                var start = _position;

                while(char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = _position -start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if(Current == '+')
            {
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);   
            }            
            if(Current == '-')
            {
                return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);   
            }
            if(Current == '*')
            {
                return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);   
            }
            if(Current == '/')
            {
                return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);   
            }
            if(Current == '(')
            {
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);   
            }
            if(Current == ')')
            {
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);   
            }

            _dianostics.Add($"Error bas Character input : {Current}");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

    class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax root)
        {
            return root.Calculate();
        }
    }
}
