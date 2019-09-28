using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface IParser
    {
        List<IToken> GetTokens(IFileSource fileSource);
    }
}