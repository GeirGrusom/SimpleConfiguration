using System;
using System.Collections;

namespace SimpleConfiguration
{
    public sealed class DictionaryConfiguration<TDictionary> : IConfiguration
        where TDictionary : IDictionary
    {
        private readonly TDictionary _section;
        public DictionaryConfiguration([NotNull] TDictionary section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            _section = section;
        }


        [return: NotNull]
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
