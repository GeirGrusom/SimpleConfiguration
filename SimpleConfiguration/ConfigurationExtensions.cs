using System;
using System.Globalization;
using System.Reflection;

namespace SimpleConfiguration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets a value from the configuration but throws if the configuration is missing.
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="key">Configuration key</param>
        /// <returns>The value fo the configuration</returns>
        /// <exception cref="ArgumentNullException">Thrown if the key is null</exception>
        /// <exception cref="MissingConfigurationKeyException">Thrown if there is no specified value for the key.</exception>
        [NotNull]
        public static  string GetValue([NotNull] this IConfiguration config, [NotNull] string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string value = config.TryGetValue(key);
            if (value == null)
            {
                throw new MissingConfigurationKeyException(key, config);
            }

            return value;
        }

        private static readonly string[] AcceptableDateTimeOffsetFormats =
        {
            "yyyy'-'MM'-'dd",
            "yyyy'-'MM'-'dd'T'HH':'mm",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
            "yyyy'-'MM'-'dd'T'HH:mm:ss'.'fff",

            "yyyy'-'MM'-'dd'Z'zzz",
            "yyyy'-'MM'-'dd'T'HH':'mm'Z'zzz",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'zzz",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'zzz",

            "yyyy'-'MM'-'dd'Z'zz",
            "yyyy'-'MM'-'dd'T'HH':'mm'Z'zz",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'zz",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'zz",

            "yyyy'-'MM'-'dd'Z'z",
            "yyyy'-'MM'-'dd'T'HH':'mm'Z'z",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'z",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'z",

        };

        /// <summary>
        /// Reads the specified key and converts it to the type specified.
        /// </summary>
        /// <typeparam name="T">Property result type</typeparam>
        /// <param name="config">Configuration source</param>
        /// <param name="key">Configuration key</param>
        /// <param name="result"></param>
        /// <remarks>This will throw if the value specified could not be understood. It only returns false if the value is not specified in the configuration.</remarks>
        /// <returns>True if value could be parsed, otherwise false.</returns>
        /// <exception cref="FormatException">Thrown if the conversion fails due to a format error</exception>
        /// <exception cref="InvalidCastException">Thrown if the type could not be converted.</exception>
        public static bool TryGetValue<T>([NotNull] this IConfiguration config, [NotNull] string key, out T result)
        {
            return TryGetValue(config, key, formatProvider: null, result: out result);
        }

        /// <summary>
        /// Tries to get the specified value and throws if the value could not be found or could not be parsed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">The conversion itself failed and can never succeed for the specified type.</exception>
        /// <exception cref="FormatException">The input format could not be understood.</exception>
        /// <exception cref="MissingConfigurationKeyException">The configuration was not found.</exception>
        [return: NotNull]
        public static T GetValue<T>([NotNull] this IConfiguration config, [NotNull] string key)
        {
            return GetValue<T>(config, key, formatProvider: null);
        }

        [return: NotNull]
        public static T GetValue<T>([NotNull] this IConfiguration config, [NotNull] string key, [CanBeNull] IFormatProvider formatProvider)
        {
            T result;
            if (config.TryGetValue(key) == null)
            {
                throw new MissingConfigurationKeyException(key, config);
            }

            bool success = TryGetValue(config, key, formatProvider, out result);
            if (success)
            {
                return result;
            }

            throw new FormatException();
        }

        /// <summary>
        /// Reads the specified key and converts it to the type specified. If the value is not specified returns false.
        /// </summary>
        /// <typeparam name="T">Property result type</typeparam>
        /// <param name="config">Configuration source</param>
        /// <param name="key">Configuration key</param>
        /// <param name="formatProvider">Format provider used for output formatting</param>
        /// <param name="result">Result of parsed expression</param>
        /// <remarks>This will throw if the value specified could not be understood. It only returns false if the value is not specified in the configuration.</remarks>
        /// <exception cref="InvalidCastException">The conversion itself failed and can never succeed for the specified type.</exception>
        /// <exception cref="FormatException">The input was not in the correct format.</exception>
        /// <returns>True if the value was found</returns>
        public static bool TryGetValue<T>([NotNull] this IConfiguration config, [NotNull] string key, [CanBeNull] IFormatProvider formatProvider, out T result)
        {
            string value = config.TryGetValue(key);

            if (value == null)
            {
                result = default(T);
                return false;
            }

            if (typeof(T) == typeof(DateTime))
            {
                return ParseDateTime(formatProvider, out result, value);
            }

            if (typeof(T) == typeof(DateTimeOffset))
            {
                return ParseDateTimeOffset(formatProvider, out result, value);
            }

            if (typeof(T) == typeof(Uri))
            {
                return ParseUri(out result, value);
            }

            if (typeof(T).IsEnum)
            {
                return ParseEnum(out result, value);
            }

            try
            {

                var convertResult = formatProvider == null 
                    ? (T)Convert.ChangeType(value, typeof(T))
                    : (T)Convert.ChangeType(value, typeof(T), formatProvider);

                result = convertResult;
                return true;
            }
            catch (InvalidCastException)
            {
                bool success = formatProvider == null 
                    ? TryParse<T>(value, out result)
                    : TryParse<T>(value, formatProvider, out result);

                if (!success)
                {
                    throw new FormatException();
                }
                return true;
            }
        }

        private static bool ParseDateTime<T>(IFormatProvider formatProvider, out T result, string value)
        {
            DateTime offsetResult;

            var parseResult = formatProvider == null
                ? DateTime.TryParseExact(value, AcceptableDateTimeOffsetFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal, out offsetResult)
                : DateTime.TryParseExact(value, AcceptableDateTimeOffsetFormats, formatProvider, DateTimeStyles.AssumeLocal,
                    out offsetResult);

            result = (T) (object) offsetResult;

            if (!parseResult)
            {
                throw new FormatException();
            }
            return true;
        }

        private static bool ParseDateTimeOffset<T>(IFormatProvider formatProvider, out T result, string value)
        {
            DateTimeOffset offsetResult;
            bool parseResult = formatProvider == null
                ? DateTimeOffset.TryParseExact(value, AcceptableDateTimeOffsetFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal, out offsetResult)
                : DateTimeOffset.TryParseExact(value, AcceptableDateTimeOffsetFormats, formatProvider,
                    DateTimeStyles.AssumeLocal, out offsetResult);
            result = (T) (object) offsetResult;

            if (!parseResult)
            {
                throw new FormatException();
            }
            return true;
        }

        private static bool ParseEnum<T>(out T result, string value)
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                result = default(T);
                throw new FormatException();
            }
            result = (T) Enum.Parse(typeof(T), value, ignoreCase: true);
            return true;
        }

        private static bool ParseUri<T>(out T result, string value)
        {
            Uri uriResult;
            if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uriResult))
            {
                result = default(T);
                throw new FormatException();
            }
            result = (T) (object) uriResult;
            return true;
        }

        private static bool TryParse<T>(string input, IFormatProvider formatProvider, out T result)
        {
            if (formatProvider == null)
            {
                throw new ArgumentNullException(nameof(formatProvider));
            }

            var t = typeof(T);

            var tryParse = t.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new [] {typeof(string), typeof(IFormatProvider), typeof(T).MakeByRefType()}, null);

            if (tryParse == null)
            {
                // Try parsing without formatProvider.
                return TryParse(input, out result);
            }
            
            var args = new object[] {input, formatProvider, default(T)};
            bool tryParseResult = (bool)tryParse.Invoke(null, args);

            if (!tryParseResult)
            {
                result = default(T);
                return false;
            }

            result = (T) args[2];
            return true;
        }

        private static bool TryParse<T>(string input, out T result)
        {
            var t = typeof(T);

            var tryParse = t.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new [] { typeof(string), typeof(T).MakeByRefType() }, null);

            if (tryParse == null)
            {
                throw new InvalidCastException();
            }

            var args = new object[] { input, default(T) };
            var tryParseResult = (bool)tryParse.Invoke(null, args);

            if (!tryParseResult)
            {
                result = default(T);
                return false;
            }

            result = (T)args[1];
            return true;
        }
    }
}
