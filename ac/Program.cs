using System;
using System.Collections.Generic;
using System.Linq;
using Aromat.CodeAnalysis;

namespace Aromat
{
    class Program
    {
        static void Main(string[] args)
        {
            var showTree = false;
            while(true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                if(line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing Expressiontree" : "Not showing Expressiontree");
                    continue;
                }   
                else if(line == "#cls")
                {
                    Console.Clear();
                    continue;
                }             

                var syntax = SyntaxTree.Parse(line);

                var color = Console.ForegroundColor;
                if(showTree)
                {
                    ShowTree(syntax, color);
                }

                if (syntax.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var dianostics in syntax.Diagnostics)
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

        private static void ShowTree(SyntaxTree syntax, ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.WriteLine("Expressiontree: ");
            PrettyPrint(syntax.Root);

            Console.ForegroundColor = color;
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

}
