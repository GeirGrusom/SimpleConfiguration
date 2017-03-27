using System;
using NSubstitute;
using NUnit.Framework;

namespace SimpleConfiguration.Tests
{
    [TestFixture(TestOf = typeof(DelegateConfiguration))]
    public class DelegateConfigurationTests
    {
        [Test]
        public void Ctor_FuncIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new DelegateConfiguration(null), Throws.ArgumentNullException);
        }

        [Test]
        public void TryGetValue_ReturnsGetValue()
        {
            // Arrange
            var getter = Substitute.For<Func<string, string>>();
            getter.Invoke("Foo").Returns("Bar");
            var d = new DelegateConfiguration(getter);

            // Act
            var result = d.TryGetValue("Foo");

            // Assert
            Assert.That(result, Is.EqualTo("Bar"));
        }

        [Test]
        public void TryGetValue_KeyNull_ThrowsArgumentNullException()
        {
            // Arrange
            var d = new DelegateConfiguration(key => "Foo");

            // Act
            // Assert
            Assert.That(() => d.TryGetValue(null), Throws.ArgumentNullException);
        }
    }
}
