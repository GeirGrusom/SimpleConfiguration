using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace SimpleConfiguration.Tests
{
    [TestFixture(TestOf = typeof(MissingConfigurationKeyException))]
    public class MissingConfigurationKeyExceptionTests
    {
        private class Configurer : IConfiguration
        {
            public override string ToString() => "FooConfig";

            public string TryGetValue(string key) => "";

        }

        [Test]
        public void Ctor_SetsKey()
        {
            // Arrange
            // Act
            var ex = new MissingConfigurationKeyException("Foo", new DelegateConfiguration(s => "Foo"));

            // Assert
            Assert.That(ex.Key, Is.EqualTo("Foo"));
        }

        [Test]
        public void Ctor_KeyNull_SetsKey()
        {
            // Arrange
            // Act
            var ex = new MissingConfigurationKeyException(null, new DelegateConfiguration(s => "Foo"));

            // Assert
            Assert.That(ex.Key, Is.Null);
        }

        [Test]
        public void Ctor_SetsConfiguration()
        {
            //Arrange
            var config = new Configurer();

            // Act
            var ex = new MissingConfigurationKeyException("Foo", config);

            // Assert
            Assert.That(ex.Configuration, Is.EqualTo(config.ToString()));
        }

        

        [Test]
        public void Deserialize_StoresKeyAndConfig()
        {
            // Arrange
            MissingConfigurationKeyException exception;
            var config = new Configurer();
            try
            {
                throw new MissingConfigurationKeyException("Foo", config);
            }
            catch (MissingConfigurationKeyException ex)
            {
                exception = ex;
            }
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, exception);
            stream.Seek(0, SeekOrigin.Begin);


            // Act
            var resultException = (MissingConfigurationKeyException)formatter.Deserialize(stream);

            // Assert
            Assert.That(resultException.Key, Is.EqualTo("Foo"));
            Assert.That(resultException.Configuration, Is.EqualTo(config.ToString()));
        }

    }
}
