using CodeAnalyzer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeParser
{
    public class CsToken : IToken
    {
        public int Position { get; set; }
        public IFileSource FileSource { get; set; }
        public int Code { get; set; }
    }
}
