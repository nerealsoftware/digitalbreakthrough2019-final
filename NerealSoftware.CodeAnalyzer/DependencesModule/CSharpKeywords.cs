using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencesModule
{
    public static class CSharpKeywords
    {
        private static Dictionary<string, int> _keywords;
        private static string[] keywordsArray = new string[] { "abstract",
"bool",
"break",
"byte",
"case",
"catch",
"char",
"class",
"const",
"continue",
"decimal",
"delegate",
"do",
"double",
"else",
"enum",
"event",
"explicit",
"extern",
"false",
"finally",
"fixed",
"float",
"for",
"foreach",
"goto",
"if",
"implicit",
"int",
"interface",
"internal",
"lock",
"long",
"namespace",
"new",
"null",
"object",
"operator",
"override",
"params",
"private",
"protected",
"public",
"return",
"sbyte",
"sealed",
"short",
"sizeof",
"stackalloc",
"static",
"string",
"struct",
"switch",
"this",
"throw",
"try",
"typeof",
"uint",
"ulong",
"ushort",
"using",
"virtual",
"void",
"volatile",
"while",
":",
"?",
"??",
"{",
"}" };
        public static object locker = new object();
        public static Dictionary<string, int> GetKeywords()
        {
            if (_keywords == null)
            {
                lock (locker)
                {
                    if (_keywords == null)
                    {
                        _keywords = new Dictionary<string, int>();
                        for (int i = 0; i < keywordsArray.Length; i++)
                        {
                            _keywords.Add(keywordsArray[i], i + 1);
                        }
                    }
                }
            }
            return _keywords;
        }

        public static Dictionary<string, int> Keywords
        {
            get { return GetKeywords(); }
        }
    }
}
