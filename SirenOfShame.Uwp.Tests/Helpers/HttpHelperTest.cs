using NUnit.Framework;
using SirenOfShame.Uwp.Watcher.Helpers;

namespace SirenOfShame.Uwp.Tests.Helpers
{
    [TestFixture]
    public class HttpHelperTest
    {
        [Test]
        public void GivenUrlWithNoParams_WhenParseQueryStringParams_ThenEmptyDictionary()
        {
            var dictionary = HttpHelper.ParseQueryStringParams("/api/ledPatterns");
            Assert.AreEqual(0, dictionary.Count);
        }

        [Test]
        public void GivenUrlWithOneParam_WhenParseQueryStringParams_ThenParamsInDictionary()
        {
            var dictionary = HttpHelper.ParseQueryStringParams("/api/ledPatterns?id=2");
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual(true, dictionary.ContainsKey("id"));
            Assert.AreEqual("2", dictionary["id"]);
        }

        [Test]
        public void GivenUrlWithTwoParams_WhenParseQueryStringParams_ThenParamsInDictionary()
        {
            var dictionary = HttpHelper.ParseQueryStringParams("/api/ledPatterns?id=2&duration=500");
            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual(true, dictionary.ContainsKey("id"));
            Assert.AreEqual("2", dictionary["id"]);
            Assert.AreEqual(true, dictionary.ContainsKey("duration"));
            Assert.AreEqual("500", dictionary["duration"]);
        }
    }
}
