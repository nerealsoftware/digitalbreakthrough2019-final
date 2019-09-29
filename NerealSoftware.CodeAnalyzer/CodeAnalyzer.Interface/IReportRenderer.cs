using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalyzer.Interface
{
    public interface IReportRenderer
    {
        string ToHtml(IProcessingResult result);
    }
}
