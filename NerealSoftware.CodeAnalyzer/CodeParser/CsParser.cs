﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeAnalyzer.Interface;
using AhoCorasick.Core;
using DependencesModule;
using Microsoft.CodeAnalysis.Text;

namespace CodeParser
{
    public class CsParser : IParser
    {
        public class Rewriter : CSharpSyntaxRewriter, IRewriter
        {
            public OffsetsStore Offsets;

            private static LiteralExpressionSyntax _literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(""));

            public Rewriter(OffsetsStore offsetsStore)
            {
                this.Offsets = offsetsStore;
            }

            public override SyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
            {
                Offsets?.StoreOffset(node.Span.Start, node.Span.Length, 2);
                return _literalExpression;
            }

            //private void UpdateOffsets(int replaceLength, TextSpan span)
            //{
            //    Offsets[span.Start - ShiftSize] = span.Start;
            //    ShiftSize += span.Length - replaceLength;
            //    Offsets[span.Start + span.Length - ShiftSize] = span.Start + span.Length;
            //}

            public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                //UpdateOffsets(2, node.Span);
                Offsets?.StoreOffset(node.Span.Start, node.Span.Length, 2);
                return _literalExpression;
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                var isBad = trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)
                            || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)
                            || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                            || trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia)
                            || trivia.IsKind(SyntaxKind.StringLiteralToken);
                if (isBad)
                {
                    //UpdateOffsets(0, trivia.Span);
                    Offsets?.StoreOffset(trivia.Span.Start, trivia.Span.Length, 0);
                    return default(SyntaxTrivia);
                }
                else
                {
                    return base.VisitTrivia(trivia);
                }
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

        public string ClearSourceText(string sourceText, OffsetsStore offsetsStore)
        {
            var tree = ParseText(sourceText);
            var root = tree.GetRoot();

            var rewriter = new Rewriter(offsetsStore);
            var result = rewriter.Visit(root);
            return result.ToFullString().ToLowerInvariant();
        }

        public List<IToken> IntGetTokens(string sourceText, IFileSource fileSource, OffsetsStore offsetsStore)
        {
            var clearText = ClearSourceText(sourceText, offsetsStore);
            SearchFsa<char> fsa = PrepareFsa();

            var tokens = new List<IToken>();
            var ep = fsa.GetEndPoint((index, context) =>
            {
                var currentWord = context.ToString();
                var token = new CsToken()
                {
                    Code = CSharpKeywords.Keywords[currentWord],
                    Position = index,
                    FileSource = fileSource,
                    StringLength = currentWord.Length
                };
                tokens.Add(token);
            });

            foreach (var ch in sourceText) ep.ProcessIncome(ch);

            return tokens;
        }

        public List<IToken> GetTokens(IFileSource fileSource)
        {
            return GetTokens(fileSource, new OffsetsStore());
        }

        public List<IToken> GetTokens(IFileSource fileSource, OffsetsStore offsetsStore)
        {
            offsetsStore = offsetsStore ?? new OffsetsStore();
            return IntGetTokens(fileSource.GetData(), fileSource, offsetsStore);
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
