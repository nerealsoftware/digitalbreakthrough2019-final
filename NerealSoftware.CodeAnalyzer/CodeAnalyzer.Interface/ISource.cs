using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalyzer.Interface
{
    interface ISource
    {
        IFileSource GetNextFile();
    }

    public interface IFileSource
    {
        long GetContentLength();
        string GetFileName();
        DateTime GetCreateDate();
        string GetData();
    }
}
