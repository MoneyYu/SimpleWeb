using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleWeb.IntegrationTest
{
    [TestClass]
    public class HealthCheckIntegrationTests
    {
        private CustomWebApplicationFactory? _factory;
        private HttpClient? _client;

        [TestInitialize]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task HealthCheck_ReturnsHealthy()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task HealthCheck_ReturnsJsonContentType()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        }

        [TestMethod]
        public async Task HealthCheck_ReturnsValidJsonStructure()
        {
            // Act
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            
            var jsonDocument = JsonDocument.Parse(content);
            jsonDocument.RootElement.TryGetProperty("status", out var statusProperty).Should().BeTrue();
            statusProperty.GetString().Should().Be("Healthy");
            
            jsonDocument.RootElement.TryGetProperty("results", out var resultsProperty).Should().BeTrue();
        }

        [TestMethod]
        public async Task HealthCheck_StatusIsHealthy()
        {
            // Act
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            var jsonDocument = JsonDocument.Parse(content);
            var status = jsonDocument.RootElement.GetProperty("status").GetString();
            status.Should().Be("Healthy");
        }
    }
}
