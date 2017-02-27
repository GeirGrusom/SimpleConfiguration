using System;
using System.Runtime.Serialization;

namespace SimpleConfiguration
{
    [Serializable]
    public sealed class MissingConfigurationKeyException : Exception
    {
        public string Key { get; }

        public string Configuration { get; }

        public MissingConfigurationKeyException()
            : base("The key could not be found in the configuration.")
        {
            Key = null;
            Configuration = null;
        }

        public MissingConfigurationKeyException(string message)
            : base(message)
        {
            Key = null;
            Configuration = null;
        }

        public MissingConfigurationKeyException(string message, [CanBeNull] string key)
            : base(message)
        {
            Key = key;
            Configuration = null;
        }

        internal MissingConfigurationKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Key = info.GetString(nameof(Key));
            Configuration = info.GetString(nameof(Configuration));
        }

        public MissingConfigurationKeyException([CanBeNull] string key, [CanBeNull] IConfiguration configuration)
            : base($"The key '{key ?? "(null)"}' could not be found in the configuration '{configuration?.ToString() ?? "(null)"}'")
        {
            Key = key;
            Configuration = configuration?.ToString();
        }

        public MissingConfigurationKeyException(string message, [CanBeNull] string key, [CanBeNull] IConfiguration configuration)
            : base(message)
        {
            Key = key;
            Configuration = configuration?.ToString();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(Configuration), Configuration);
        }
    }
}
