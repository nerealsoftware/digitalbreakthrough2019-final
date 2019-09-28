using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeParser
{
    public interface IRewriter
    {
        SyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node);
        SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node);
        SyntaxTrivia VisitTrivia(SyntaxTrivia trivia);
    }
}