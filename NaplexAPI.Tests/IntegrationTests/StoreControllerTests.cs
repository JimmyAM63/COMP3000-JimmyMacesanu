using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class StoreControllerTests
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private readonly CustomWebApplicationFactory _factory;

        public StoreControllerTests(ITestOutputHelper output)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task GetStores_ShouldReturnAllSeededStores()
        {
            var response = await _client.GetAsync("/Stores");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine("[GetStores] Response:");
            _output.WriteLine(content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("Newton_Abbot");
            content.Should().Contain("Exmouth");
        }

        [Fact]
        public async Task GetStoreById_ShouldReturnCorrectStore()
        {
            var response = await _client.GetAsync("/Stores/1");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine("[GetStoreById] Response:");
            _output.WriteLine(content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("Newton_Abbot");
        }

        [Fact]
        public async Task GetUserStores_ShouldReturnStoresLinkedToUser()
        {
            var response = await _client.GetAsync("/Stores/user/u123/stores");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine("[GetUserStores] Response:");
            _output.WriteLine(content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("Newton_Abbot");
        }

        [Fact]
        public async Task GetStoreById_ShouldReturnNotFound_WhenIdInvalid()
        {
            var response = await _client.GetAsync("/Stores/999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}