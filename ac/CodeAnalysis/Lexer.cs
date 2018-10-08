using System.Collections.Generic;

namespace Aromat.CodeAnalysis
{
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

                while(char.IsDigit(Current) || Current == ',')
                {
                    Next();
                }

                var length = _position -start;
                var text = _text.Substring(start, length);

                if(!decimal.TryParse(text, out var number))
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
}