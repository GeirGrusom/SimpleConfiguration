using System;

namespace SimpleConfiguration
{
    public sealed class DelegateConfiguration : IConfiguration
    {
        private readonly Func<string, string> _getter;

        public DelegateConfiguration([NotNull]Func<string, string> getter)
        {
            if (getter == null)
            {
                throw new ArgumentNullException(nameof(getter));
            }
            _getter = getter;
        }

        [return: NotNull]
        public string TryGetValue([NotNull] string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _getter(key);
        }
    }
}
