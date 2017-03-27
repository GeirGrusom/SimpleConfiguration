using System.Collections.Generic;
using NUnit.Framework;

namespace SimpleConfiguration.Tests
{
    [TestFixture(TestOf = typeof(DictionaryConfiguration<>))]
    public class DictionaryConfigurationTests
    {

        [Test]
        public void Ctor_DictionaryNull_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new DictionaryConfiguration< Dictionary<string, string> >(null), Throws.ArgumentNullException);
        }

        [Test]
        public void TryGetValue_ReturnsDictionaryValue()
        {
            // Arrange
            var conf = new DictionaryConfiguration< Dictionary<string, string> >(new Dictionary<string, string> { ["Foo"] = "Bar" });

            // Act
            var result = conf.TryGetValue("Foo");

            // Assert
            Assert.That(result, Is.EqualTo("Bar"));
        }

        [Test]
        public void TryGetValue_KeyIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var conf = new DictionaryConfiguration<Dictionary<string, string>>(new Dictionary<string, string>());

            // Act
            // Assert
            Assert.That(() => conf.TryGetValue(null), Throws.ArgumentNullException);
        }
    }
}
