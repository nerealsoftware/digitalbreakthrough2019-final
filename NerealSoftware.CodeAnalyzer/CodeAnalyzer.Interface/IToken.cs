using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalyzer.Interface
{
    public interface IToken
    {
        int Position { get; set; }
        IFileSource FileSource { get; set; }
        int Code { get; set; }
    }
}
