using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencesModule
{
    public static class CSharpKeywords
    {
        private static string[] _keywords = new string[] {
            "abstract", "bool", "break", "byte", "case", "catch", "char", "class", "const", "continue",
            "decimal", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false",
            "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "int", "interface",
            "internal", "lock", "long", "namespace", "new", "null", "object", "operator",
            "override", "params", "private", "protected", "public", "return", "sbyte",
            "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "throw",
            "try", "typeof", "uint", "ulong", "ushort",  "using", "static", "virtual",
            "void", "volatile", "while", ":", "?", "??", "{", "}"};

        public static string[] Keywords
        {
            get { return _keywords; }
        }
    }
}
