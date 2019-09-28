namespace CodeAnalyzer.Interface
{
    public interface ISimilarBlock
    {
        IToken File1Start { get; }
        IToken File1End { get; }
        IToken File2Start { get; }
        IToken File2End { get; }
    }
}