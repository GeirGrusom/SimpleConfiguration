using System;
using NSubstitute;
using NUnit.Framework;

namespace SimpleConfiguration.Tests
{
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
}
