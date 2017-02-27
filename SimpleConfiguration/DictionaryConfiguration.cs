using System;
using System.Collections;

namespace SimpleConfiguration
{
    /// <summary>
    /// Implementation of IConfiguration for normal dictionaries.
    /// </summary>
    /// <typeparam name="TDictionary"></typeparam>
    public sealed class DictionaryConfiguration<TDictionary> : IConfiguration
        where TDictionary : IDictionary
    {
        private readonly TDictionary _section;
        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="section">Dictionary containing values for this configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if section is null</exception>
        public DictionaryConfiguration([NotNull] TDictionary section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            _section = section;
        }

        /// <inheritdoc />
        [return: CanBeNull]
        public string TryGetValue([NotNull]string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _section[key]?.ToString();
        }
    }
}
