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
using CodeAnalyzer.Interface;
using AhoCorasick.Core;
using DependencesModule;

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

        public List<IToken> IntGetTokens(string sourceText, IFileSource fileSource)
        {
            var clearText = ClearSourceText(sourceText);
            SearchFsa<char> fsa = PrepareFsa();

            var tokens = new List<IToken>();
            var ep = fsa.GetEndPoint((index, context) =>
            {
                var currentWord = context.ToString();
                var token = new CsToken() { Code = CSharpKeywords.Keywords[currentWord], Position = index, FileSource = fileSource };
                tokens.Add(token);
            });

            foreach (var ch in sourceText) ep.ProcessIncome(ch);

            return tokens;
        }

        public List<IToken> GetTokens(IFileSource fileSource)
        {
            return IntGetTokens(fileSource.GetData(), fileSource);
        }

        private static SearchFsa<char> _fsa = null;
        private static SearchFsa<char> PrepareFsa()
        {
            if (_fsa == null)
            {
                _fsa = new SearchFsa<char>();
                foreach (var word in CSharpKeywords.Keywords) _fsa.Add(word.Key);
                _fsa.Prepare();
            }
            return _fsa;
        }
    }
}
