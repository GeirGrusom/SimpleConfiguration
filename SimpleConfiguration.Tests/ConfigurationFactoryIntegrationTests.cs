using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using NUnit.Framework;

namespace SimpleConfiguration.Tests
{
    [TestFixture(TestOf = typeof(ConfigurationFactory))]
    public class ConfigurationFactoryIntegrationTests
    {
        private static readonly object[] DictionaryTypes =
        {
            new Dictionary<string, string>(),
            new ListDictionary(), 
            new Hashtable(),
            new HybridDictionary(),
            new OrderedDictionary(), 
            new SortedList(), 
            new NameValueCollection(),
            new AppSettingsSection(),
        };

        [TestCaseSource(nameof(DictionaryTypes))]
        public void ValueNotDefined_ReturnsNull(dynamic dictionary)
        {
            // Arrange
            var conf = (IConfiguration)ConfigurationFactory.Create(dictionary);

            // Act
            var value = conf.TryGetValue("Foo");

            // Assert
            Assert.That(value, Is.Null);
        }

        private static readonly Func<object>[] DictionaryFactories =
        {
            () => new Dictionary<string, string>(),
            () => new ListDictionary(),
            () => new Hashtable(),
            () => new HybridDictionary(),
            () => new OrderedDictionary(),
            () => new SortedList(),
            () => new NameValueCollection(),
        };

        [TestCaseSource(nameof(DictionaryFactories))]
        public void TryGetValue_Returns_ValueDefined(Func<object> dictionaryFactory)
        {
            // Arrange
            dynamic dictionary = dictionaryFactory();
            dictionary.Add("Foo", "Bar");
            var conf = (IConfiguration)ConfigurationFactory.Create(dictionary);

            // Act
            var value = conf.TryGetValue("Foo");

            // Assert
            Assert.That(value, Is.EqualTo("Bar"));
        }

        [Test]
        public void TryGetValue_For_AppSettingsSection_ReturnsSpecifiedValue()
        {
            // Arrange
            var settings = new AppSettingsSection();
            settings.Settings.Add("Foo", "Bar");
            var conf = ConfigurationFactory.Create(settings);

            // Act
            var result = conf.TryGetValue("Foo");

            // Assert
            Assert.That(result, Is.EqualTo("Bar"));
        }
    }
}
