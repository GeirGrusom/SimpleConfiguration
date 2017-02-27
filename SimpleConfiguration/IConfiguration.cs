using System;

namespace SimpleConfiguration
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    public sealed class CanBeNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    public sealed class NotNullAttribute : Attribute
    {
    }

    public interface IConfiguration
    {
        /// <summary>
        /// Gets the value associated with the key or null if the key could not be found.
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>String value of the configuration or null if it could not be found.</returns>
        [return: CanBeNull] string TryGetValue([NotNull] string key);
    }
}
