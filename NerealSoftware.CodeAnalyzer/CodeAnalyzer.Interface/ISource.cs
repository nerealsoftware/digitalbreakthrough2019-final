using System;
using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface ISource
    {
        IEnumerable<IFileSource> GetFiles();
    }

    public interface IFileSource
    {
        long GetContentLength();
        string GetFileName();
        DateTime GetCreateDate();
        string GetData();
    }
}
