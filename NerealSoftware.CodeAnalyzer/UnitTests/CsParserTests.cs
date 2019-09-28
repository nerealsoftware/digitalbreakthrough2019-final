using System;
using CodeAnalyzer.Interface;
using CodeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        [TestMethod]
        public void IntGetTokens()
        {
            var src = TestHelper.ReadResource("UnitTests.Res.AhoKorasikAlgorithm.txt");
            var parser = new CsParser();
            var tokens = parser.IntGetTokens(src, null);
            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count > 0);
        }

        [TestMethod]
        public void GetTokens()
        {
            IFileSource fs = Mock.Of<IFileSource>();

            var src = TestHelper.ReadResource("UnitTests.Res.AhoKorasikAlgorithm.txt");
            var fs2 = new Mock<IFileSource>();
            fs2.Setup(x => x.GetData()).Returns(() => src);
            var parser = new CsParser();
            var tokens = parser.GetTokens(fs2.Object);
            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count > 0);
        }
    }
}