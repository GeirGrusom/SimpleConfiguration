﻿using System;
using System.Runtime.Serialization;

namespace SimpleConfiguration
{
    /// <summary>
    /// When this exception is thrown it indicates that a configuration key is missing from the configuration set.
    /// </summary>
    [Serializable]
    public sealed class MissingConfigurationKeyException : Exception
    {
        /// <summary>
        /// The missing key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Description of the configuration source.
        /// </summary>
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

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Key), Key);
            info.AddValue(nameof(Configuration), Configuration);
        }
    }
}
