using System.Linq;
using NUnit.Framework;
using ResiLab.MailFilter.Learning.Parser;

namespace ResiLab.MailFilter.Tests.Learning.Parser {
    [TestFixture]
    public class UrlParserTests {
        [Test]
        public void Should_Detect_A_Single_Url() {
            var text = "http://example.tld";

            var result = UrlParser.Parse(text);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("http://example.tld", result.FirstOrDefault());
        }

        [Test]
        public void Should_Detect_A_Url_In_A_Text() {
            var text = "This is a Spam Mail\n\rhttp://example.tld\r\nTest";

            var result = UrlParser.Parse(text);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("http://example.tld", result.FirstOrDefault());
        }

        [Test]
        public void Should_Detect_Two_Urls_In_A_Text() {
            var text = "http://example.tld https://exampletwo.tld";

            var result = UrlParser.Parse(text);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("http://example.tld", result.ElementAt(0));
            Assert.AreEqual("https://exampletwo.tld", result.ElementAt(1));
        }

        [Test]
        public void Should_Detect_Url_In_Real_Spam_Mail_Text() {
            var text =
                "<html>\r\n<body>\r\nHi!<br>\r\n<br>\r\nJeder um dich herum wird flachgelegt! Wir haben viele neue Mitglieder aus deiner Gegend. Viele warten auf einen Chatpartner wie dich!<br>\r\n<br>\r\nAnmeldung ist kostenlos, also warum nicht gleich ausprobieren?<br>\r\nHIER ist dein Link fur angenehme Zeiten:<br>\r\n<br>\r\n<a href=\"http://spam.spam.tld/gallery.php?g=136&36sBCqLQN9AQrpio=17tJDbUh&3AJ=6KM&2q=2b3v\">Klick Hier, um dich anzumelden</a><br>\r\n<br>\r\nViel Spa?!<br>\r\n</body>\r\n</html>";

            var result = UrlParser.Parse(text);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("http://spam.spam.tld/gallery.php?g=136&36sBCqLQN9AQrpio=17tJDbUh&3AJ=6KM&2q=2b3v",
                result.FirstOrDefault());
        }
    }
}