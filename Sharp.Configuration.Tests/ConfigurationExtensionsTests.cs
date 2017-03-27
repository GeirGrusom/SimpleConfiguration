using System;
using System.Globalization;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace SimpleConfiguration.Tests
{
    [TestFixture(TestOf = typeof(ConfigurationExtensions))]
    public class ConfigurationExtensionsTests
    {
        [Test]
        public void GetValue_String_DoesNotExist_ThrowsMissingConfigurationKeyException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns((string)null);

            // Act
            // Assert
            Assert.That(() => config.GetValue("Foo"), Throws.InstanceOf<MissingConfigurationKeyException>().With.Property("Key").EqualTo("Foo"));
        }

        private static readonly object[] GetValueFormatProviderTestCases = 
        {
            new object[] { "2001-02-03T12:39:23.987Z+1", new DateTimeOffset(2001, 2, 3, 12, 39, 23, 987, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23Z+1", new DateTimeOffset(2001, 2, 3, 12, 39, 23, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39Z+1", new DateTimeOffset(2001, 2, 3, 12, 39, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03Z+1", new DateTimeOffset(2001, 2, 3, 0, 0, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23.987Z+01", new DateTimeOffset(2001, 2, 3, 12, 39, 23, 987, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23Z+01", new DateTimeOffset(2001, 2, 3, 12, 39, 23, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39Z+01", new DateTimeOffset(2001, 2, 3, 12, 39, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03Z+01", new DateTimeOffset(2001, 2, 3, 0, 0, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23.987Z+01:30", new DateTimeOffset(2001, 2, 3, 12, 39, 23, 987, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03T12:39:23Z+01:30", new DateTimeOffset(2001, 2, 3, 12, 39, 23, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03T12:39Z+01:30", new DateTimeOffset(2001, 2, 3, 12, 39, 0, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03Z+01:30", new DateTimeOffset(2001, 2, 3, 0, 0, 0, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03T12:39:23.987", new DateTime(2001, 2, 3, 12, 39, 23, 987) },
            new object[] { "2001-02-03T12:39:23", new DateTime(2001, 2, 3, 12, 39, 23) },
            new object[] { "2001-02-03T12:39", new DateTime(2001, 2, 3, 12, 39, 0) },
            new object[] { "2001-02-03", new DateTime(2001, 2, 3, 0, 0, 0) },
            new object[] { "1.0", 1.0 },
            new object[] { "1", 1 },
            new object[] { "1.0", 1.0f },
            new object[] { "true", true},
            new object[] { "false", false},
            new object[] { "True", true},
            new object[] { "False", false},
            new object[] { "http://localhost", new Uri("http://localhost", UriKind.Absolute)},
            new object[] { "localhost", new Uri("localhost", UriKind.Relative)}
        };

        [TestCaseSource(nameof(GetValueFormatProviderTestCases))]
        public void GetValue_T_FormatProvider_ReturnsExpectedValue(string value, dynamic expectedValue)
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns(value);
            var method = typeof(ConfigurationExtensions).GetMethods().Single(x => x.Name == "GetValue" && x.IsGenericMethodDefinition && x.GetParameters().Length == 3);
            var meth = method.MakeGenericMethod(expectedValue.GetType());
            

            // Act
            var result = meth.Invoke(null, new object[] {config, "Foo", CultureInfo.InvariantCulture});

            // Assert
            Assert.That(result, Is.EqualTo(expectedValue));

        }

        public struct Parseable
        {
            public int Value { get; set; }
            public static bool TryParse(string input, IFormatProvider provider, out Parseable result)
            {
                result = new Parseable  { Value = 1 };
                return true;
            }

            public static bool TryParse(string input, out Parseable result)
            {
                result = new Parseable { Value = 2 };
                return true;
            }
        }

        [Test]
        public void GetValue_InvokesTryParse_FormatProvider_Success()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("Bar");

            // Act
            var result = config.GetValue<Parseable>("Foo", CultureInfo.InvariantCulture);

            // Assert
            Assert.That(result.Value, Is.EqualTo(1));
        }

        [Test]
        public void GetVaue_InvokesTryParse_Success()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("Bar");

            // Act
            var result = config.GetValue<Parseable>("Foo");

            // Assert
            Assert.That(result.Value, Is.EqualTo(2));
        }


        private static readonly object[] GetValueTestCases =
        {
            new object[] { "2001-02-03T12:39:23.987Z+1", new DateTimeOffset(2001, 2, 3, 12, 39, 23, 987, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23Z+1", new DateTimeOffset(2001, 2, 3, 12, 39, 23, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39Z+1", new DateTimeOffset(2001, 2, 3, 12, 39, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03Z+1", new DateTimeOffset(2001, 2, 3, 0, 0, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23.987Z+01", new DateTimeOffset(2001, 2, 3, 12, 39, 23, 987, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23Z+01", new DateTimeOffset(2001, 2, 3, 12, 39, 23, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39Z+01", new DateTimeOffset(2001, 2, 3, 12, 39, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03Z+01", new DateTimeOffset(2001, 2, 3, 0, 0, 0, new TimeSpan(0, 1, 0, 0)) },
            new object[] { "2001-02-03T12:39:23.987Z+01:30", new DateTimeOffset(2001, 2, 3, 12, 39, 23, 987, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03T12:39:23Z+01:30", new DateTimeOffset(2001, 2, 3, 12, 39, 23, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03T12:39Z+01:30", new DateTimeOffset(2001, 2, 3, 12, 39, 0, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03Z+01:30", new DateTimeOffset(2001, 2, 3, 0, 0, 0, new TimeSpan(0, 1, 30, 0)) },
            new object[] { "2001-02-03T12:39:23.987", new DateTime(2001, 2, 3, 12, 39, 23, 987) },
            new object[] { "2001-02-03T12:39:23", new DateTime(2001, 2, 3, 12, 39, 23) },
            new object[] { "2001-02-03T12:39", new DateTime(2001, 2, 3, 12, 39, 0) },
            new object[] { "2001-02-03", new DateTime(2001, 2, 3, 0, 0, 0) },
            new object[] { "true", true},
            new object[] { "false", false},
            new object[] { "True", true},
            new object[] { "False", false},
            new object[] { "http://localhost", new Uri("http://localhost", UriKind.Absolute)},
            new object[] { "localhost", new Uri("localhost", UriKind.Relative)}
        };

        [TestCaseSource(nameof(GetValueTestCases))]
        public void GetValue_T_ReturnsExpectedValue(string value, object expectedValue)
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns(value);
            var method = typeof(ConfigurationExtensions).GetMethods().Single(x => x.Name == "GetValue" && x.IsGenericMethodDefinition && x.GetParameters().Length == 2);
            var meth = method.MakeGenericMethod(expectedValue.GetType());

            // Act
            var result = meth.Invoke(null, new object[] { config, "Foo" });

            // Assert
            Assert.That(result, Is.EqualTo(expectedValue));

        }

        [Test]
        public void GetValue_String_Exists_ReturnsValue()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("Bar");

            // Act
            var result = config.GetValue("Foo");

            // Assert
            Assert.That(result, Is.EqualTo("Bar"));
        }

        [Test]
        public void GetValue_Unparseable_ThrowsInvalidCastException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            Assert.That(() => config.GetValue<EventArgs>("Foo"), Throws.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void GetValue_FormatProvider_Unparseable_ThrowsInvalidCastException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            Assert.That(() => config.GetValue<EventArgs>("Foo", CultureInfo.InvariantCulture), Throws.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void GetValue_InvalidValue_ThrowsFormatException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            Assert.That(() => config.GetValue<int>("Foo"), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void GetValue_FormatProvider_InvalidValue_ThrowsInvalidCastException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            Assert.That(() => config.GetValue<int>("Foo", CultureInfo.InvariantCulture), Throws.InstanceOf<FormatException>());
        }

        private enum FooBar
        {
            Bar
        }

        [Test]
        public void GetValue_Enum_ReturnsValue()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("Bar");

            // Act
            var result = config.GetValue<FooBar>("Foo");

            // Assert
            Assert.That(result, Is.EqualTo(FooBar.Bar));
        }

        [Test]
        public void GetValue_Enum_MissingDefinition_ThrowsFormatException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("123");

            // Act
            // Assert
            Assert.That(() => config.GetValue<FooBar>("Foo"), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void TryGetValue_MissingValue_ReturnsFalse()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns((string)null);

            // Act
            int result;
            var success = config.TryGetValue("Foo", out result);

            // Assert
            Assert.That(success, Is.False);
        }

        [Test]
        public void TryGetValue_InvalidValue_ThrowsFormatException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            int result;
            Assert.That(() => config.TryGetValue("Foo", out result), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void TryGetValue_Unparseable_ThrowsInvalidCastException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            EventArgs result;
            Assert.That(() => config.TryGetValue("Foo", out result), Throws.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void TryGetValue_FormatProvider_MissingValue_ReturnsFalse()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns((string)null);

            // Act
            int result;
            var success = config.TryGetValue("Foo", CultureInfo.InvariantCulture, out result);

            // Assert
            Assert.That(success, Is.False);
        }

        [Test]
        public void TryGetValue_FormatProvider_InvalidValue_ThrowsFormatException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            int result;
            Assert.That(() => config.TryGetValue("Foo", CultureInfo.InvariantCulture, out result), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void TryGetValue_FormatProvidert_Unparseable_ThrowsInvalidCastException()
        {
            // Arrange
            var config = Substitute.For<IConfiguration>();
            config.TryGetValue("Foo").Returns("abc");

            // Act
            // Assert
            EventArgs result;
            Assert.That(() => config.TryGetValue("Foo", CultureInfo.InvariantCulture, out result), Throws.InstanceOf<InvalidCastException>());
        }
    }
}
