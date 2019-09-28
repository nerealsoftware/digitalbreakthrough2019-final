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
        public void OffsetsSimple()
        {
            var offsetsStore = new OffsetsStore();
            offsetsStore.StoreOffset(2,4,2);
            offsetsStore.StoreOffset(5,0,0);
            offsetsStore.StoreOffset(9,5,2);
            offsetsStore.StoreOffset(13,0,0);

            Assert.AreEqual(offsetsStore.RecoverOffset(2), 2);
            Assert.AreEqual(offsetsStore.RecoverOffset(3), 5);
            Assert.AreEqual(offsetsStore.RecoverOffset(7), 9);
            Assert.AreEqual(offsetsStore.RecoverOffset(8), 13);
            Assert.AreEqual(offsetsStore.RecoverOffset(1), 0);
            Assert.AreEqual(offsetsStore.RecoverOffset(6), 6);
        }

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
            var offsetStore = new OffsetsStore();
            var src = TestHelper.ReadResource("UnitTests.Res.AhoKorasikAlgorithm.txt");
            var parser = new CsParser();
            Assert.IsFalse(string.IsNullOrEmpty(src));
            var result = parser.ClearSourceText(src, offsetStore);
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Console.WriteLine(result);
        }

        [TestMethod]
        public void IntGetTokens()
        {
            var offsetStore = new OffsetsStore();
            var src = TestHelper.ReadResource("UnitTests.Res.AhoKorasikAlgorithm.txt");
            var parser = new CsParser();
            var tokens = parser.IntGetTokens(src, null, offsetStore);
            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count > 0);
        }

        [TestMethod]
        public void GetTokens()
        {
            var offsetStore = new OffsetsStore();
            IFileSource fs = Mock.Of<IFileSource>();

            var src = TestHelper.ReadResource("UnitTests.Res.AhoKorasikAlgorithm.txt");
            var fs2 = new Mock<IFileSource>();
            fs2.Setup(x => x.GetData()).Returns(() => src);
            var parser = new CsParser();
            var tokens = parser.GetTokens(fs2.Object, offsetStore);
            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count > 0);
        }
    }
}