# Configuration

This library intends to provide simple abstraction to read from different configuration files.
Primarily dictionary-like objects are supported like AppSettings in App.config or Web.config.

## Why?

An important aspect of configuration is standardizing the values. It's hard to make sure that the correct values are actually written to the files
but this intends to make sure that the correct names are used across the application and that converting to appropriate datatypes is supported.

## How?

The basic building block of this library is the `IConfiguration` interface. This specifies a single method:

```csharp
[return: CanBeNull] string TryGetValue([NotNull] string key);
```

This is called `TryGetValue`  to hammer home the point that it returns `null` if no value is found. Implementations are also supposed to throw `MissingConfigurationKeyException` if `key` is not found.

You can create some predefined implementations of this interface using `ConfigurationFactory.Create` which has a couple of useful overloads
that enables it to work with `ConfigurationManager.AppSettings` and a bunch of other types. You can also get an implementation for `MachineConfig` fairly easily.

The point after this is to rely on extension methods. You create a static class (or multiple static classes) with extension methods for `IConfiguration` and use `GetValue` and `GetValue<T>` to read the values.

This enables you to fairly easily write tests for the configuration files, and to substitute the configuration easily in your own tests. This is the reason why `TryGetValue` doesn't return `bool`: it would be way more convoluted in a mock scenario.

## Methods

### string GetValue(string key)

This tries to find a value for the specified key and throws if the key is not found or parsing failed.

### T GetValue<T>(string key, IFormatProvider)

Tries to find a value for the specified key and convert it using the specified `IFormatProvider`. Throws if the value is not found or parsing failed.

### bool TryGetValue<T>(string key, out T result)

Tries to get the configuration and parse it using the specified type. Note that this throws if the value could not be converted. It only returns false if the value could not be found.
It will use invariant culture for parsing (DateTime and DateTimeOffset will use a subset of ISO-8601).


### bool TryGetValue<T>(string key, IFormatProvider, out T result)

Tries to get the configuration and parse it using the specified type and format provider. Note that this throws if the value could not be converted. It only returns false if the value could not be found.


## Example



	// Helper structure for theoretical separate configuration subsection.
	public struct HostConfiguration : IHostConfiguration
	{
		private readonly IConfiguration _configuration;

		IConfiguration IHostConfiguration.Configuration => _configuration;

		public HostConfiguration(IConfiguration configuration)
		{
			_configuration = configuration;
		}
	}

	// Boilerplate interface for HostConfiguration subsection
	public interface IHostConfiguration
	{
		IConfiguration Configuration { get; }
	}

	public static class HostConfigurationExtensions
	{
		public static Uri Uri(this IHostConfiguration config) => config.Configuration.GetValue<Uri>("Host.Uri");
	}

	public static class AppConfigurationExtensions
	{
		public static int GetSessionTimeout(this IConfiguration configuration) => configuration.GetValue<int>("SessionTimeout");

		public static IHostConfiguration HostConfiguration(this IConfiguration config) => new HostConfiguration(config);
	}

	[TestFixture]
	public class ExampleTest
	{
		[Test]
		public void Configuration_SessionTimeout()
		{
			// Arrange
			var config = Substitute.For<IConfiguration>();
			config.TryGetValue("SessionTimeout").Returns("5000");

			// Act
			var timeout = config.GetSessionTimeout();

			// Assert
			Assert.That(timeout, Is.EqualTo(5000));
		}

		[Test]
		public void HostConfiguration_Uri_ReturnsAbsoluteUri()
		{
			// Arrange
			var config = Substitute.For<IConfiguration>();
			config.TryGetValue("Host.Uri").Returns("http://localhost");

			// Act
			var hostUri = config.HostConfiguration().Uri();

			// Assert
			Assert.That(hostUri, Is.EqualTo(new Uri("http://localhost", UriKind.Absolute)));
		}
	}


## Remarks

Note that `DateTime` and `DateTimeOffset` use ISO-8601 date and time specification. All values use `InvariantCulture` unless otherwise specified.

The parsing routine will first check for a few specific cases (such as DateTime, DateTimeOffset and Uri)
 and then it will try `Convert.ChangeType` before looking for a `public static bool TryParse(string input, out T result)` or
  `public static bool TryParse(string input, IFormatProvider formatProvider, out T result)` on the target type.
