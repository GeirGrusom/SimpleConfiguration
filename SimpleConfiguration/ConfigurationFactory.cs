using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

namespace SimpleConfiguration
{
    /// <summary>
    /// Provides factory methods to easily create configuration objects for the most common configuration sources.
    /// </summary>
    public static class ConfigurationFactory
    {
        /// <summary>
        /// Creates a configuration for the specified <see cref="IDictionary"/> source
        /// </summary>
        /// <typeparam name="TDictionary">Dictionary type parameter</typeparam>
        /// <param name="dictionary">Dictionary source</param>
        /// <returns>Configuration object using the dictionary as a source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="dictionary"/> is null</exception>
        [return: NotNull]
        public static IConfiguration Create<TDictionary>([NotNull] TDictionary dictionary)
            where TDictionary : IDictionary
        {
            return new DictionaryConfiguration<TDictionary>(dictionary);
        }

        /// <summary>
        /// Creates a configuration for the specified <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="collection"><see cref="NameValueCollection"/> source</param>
        /// <returns>Configuration object using the <see cref="NameValueCollection"/> as a source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="collection"/> is null</exception>
        [return: NotNull]
        public static IConfiguration Create([NotNull] NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            return new DelegateConfiguration(collection.Get);
        }

        /// <summary>
        /// Creates a configuration for the specified <see cref="AppSettingsSection"/>
        /// </summary>
        /// <param name="section"><see cref="AppSettingsSection"/> source</param>
        /// <returns>Cofiguration object using the <see cref="AppSettingsSection"/> as a source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="section"/> is null</exception>
        [return: NotNull]
        public static IConfiguration Create([NotNull] AppSettingsSection section)
        {
            return Create(section.Settings);
        }

        /// <summary>
        /// Creates a configuration for the specified <see cref="KeyValueConfigurationCollection"/>
        /// </summary>
        /// <param name="section"><see cref="KeyValueConfigurationCollection"/> source</param>
        /// <returns>Configuration object using the <see cref="KeyValueConfigurationCollection"/> as a source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="section"/> is null</exception>
        [return: NotNull]
        public static IConfiguration Create([NotNull] KeyValueConfigurationCollection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            return new DelegateConfiguration(key => section[key]?.Value);
        }


        /// <summary>
        /// Creates a configuration for the specified delegate
        /// </summary>
        /// <param name="func">Delegate used to retrieve configuration values</param>
        /// <returns>Configuration object using the <see cref="Func{String,String}"/> as a source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="func"/> is null</exception>
        [return: NotNull]
        public static IConfiguration Create([NotNull] Func<string, string> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            return new DelegateConfiguration(func);
        }
    }
}