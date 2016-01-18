using NUnit.Framework;
using ResiLab.MailFilter.Infrastructure;

namespace ResiLab.MailFilter.Tests {
    [TestFixture]
    public class CryptographyTests {
        [Test]
        public void Should_Encrypt_And_Decrypt_String() {
            var str = "Hello World";

            var encrypted = Cryptography.Encrypt(str);
            Assert.AreNotEqual("Hello World", encrypted);

            var decrypted = Cryptography.Decrypt(encrypted);
            Assert.AreEqual("Hello World", decrypted);
        }
    }
}