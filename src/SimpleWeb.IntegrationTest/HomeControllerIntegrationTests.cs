using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleWeb.IntegrationTest
{
    [TestClass]
    public class HomeControllerIntegrationTests
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
        public async Task Index_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
        }

        [TestMethod]
        public async Task Index_ContainsExpectedContent()
        {
            // Act
            var response = await _client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("SimpleWeb");
        }

        [TestMethod]
        public async Task Index_WithAuthenticationHeader_DisplaysUserName()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            request.Headers.Add("X-MS-CLIENT-PRINCIPAL-NAME", "testuser@example.com");

            // Act
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("testuser@example.com");
        }

        [TestMethod]
        public async Task Index_WithoutAuthenticationHeader_ShowsNotLoggedIn()
        {
            // Act
            var response = await _client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Not login yet");
        }

        [TestMethod]
        public async Task Privacy_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/Home/Privacy");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
        }

        [TestMethod]
        public async Task Privacy_ContainsPrivacyContent()
        {
            // Act
            var response = await _client.GetAsync("/Home/Privacy");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Privacy Policy");
        }

        [TestMethod]
        public async Task Upload_Get_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/Home/Upload");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.ToString().Should().Contain("text/html");
        }

        [TestMethod]
        public async Task Error_ReturnsCorrectErrorPage()
        {
            // Act
            var response = await _client.GetAsync("/Home/Error");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Error");
        }

        [TestMethod]
        public async Task NonExistentPage_Returns404()
        {
            // Act
            var response = await _client.GetAsync("/NonExistent/Page");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            return (IHtmlDocument)document;
        }
    }
}
