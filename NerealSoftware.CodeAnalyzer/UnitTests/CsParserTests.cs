using System;
using CodeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class CsParserTests
    {
        [TestMethod]
        public void ParseText()
        {
            var src = TestHelper.ReadResource("UnitTests.Res.AhoKorasikAlgorithm.txt");
            var parser = new CsParser();
            var tree = parser.ParseText(src);
            Assert.IsNotNull(tree);
        }

        [TestMethod]
        public void ClearSourceText()
        {
            var src = TestHelper.ReadResource("UnitTests.Res.AhoKorasikAlgorithm.txt");
            var parser = new CsParser();
            Assert.IsFalse(string.IsNullOrEmpty(src));
            var result = parser.ClearSourceText(src);
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Console.WriteLine(result);
        }
    }
}