using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeParser
{
    public class CsParser
    {
        public class Rewriter : CSharpSyntaxRewriter
        {
            private static LiteralExpressionSyntax _literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(""));

            public override SyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
            {
                return _literalExpression;
            }

            public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                
                return _literalExpression;
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                var isBad = trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)
                            || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)
                            || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                            || trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia)
                            || trivia.IsKind(SyntaxKind.StringLiteralToken);
                return isBad ? default(SyntaxTrivia) : base.VisitTrivia(trivia);
            }
        }

        public SyntaxTree ParseText(string text)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            return syntaxTree;
        }

        public SyntaxTree ParseFile(string fileName)
        {
            return ParseFile(fileName, Encoding.UTF8);
        }

        public SyntaxTree ParseFile(string fileName, Encoding encoding)
        {
            var text = File.ReadAllText(fileName, encoding);
            return ParseText(text);
        }

        public string ClearSourceText(string sourceText)
        {
            var tree = ParseText(sourceText);
            var root = tree.GetRoot();

            var rewriter = new Rewriter();
            var result = rewriter.Visit(root);
            return result.ToFullString().ToLowerInvariant();
        }
    }
}
