using System.Collections.Generic;
using CodeAnalyzer.Interface;

namespace CodeAnalyzer.Utils
{
    public class FileLineExtractor
    {
        public string[] ExtractLines(IFileSource file, int startPosition, int endPosition)
        {
            var data = file.GetData();
            var i = startPosition;
            while (i > 0 && data[i - 1] != '\n' && data[i - 1] != '\r') i--;

            var result = new List<string>();
            while (i <= endPosition && i < data.Length)
            {
                var s = i;
                while (i < data.Length && data[i] != '\n' && data[i] != '\r') i++;

                result.Add(data.Substring(s, i - s));
                if (i < data.Length)
                {
                    var c = data[i];
                    i++;
                    if (i < data.Length && (data[i] == '\n' || data[i] == '\r') && data[i] != c) i++;
                }
            }

            return result.ToArray();
        }
    }
}