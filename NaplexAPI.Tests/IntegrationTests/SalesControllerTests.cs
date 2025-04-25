using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTests.Setup;
using NaplexAPI.Models.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class SalesControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringDateTimeConverter() }
        };

        private static HttpContent ToJsonContent<T>(T obj) =>
            new StringContent(JsonSerializer.Serialize(obj, _jsonOptions), Encoding.UTF8, "application/json");

        public SalesControllerTests(ITestOutputHelper output)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task CreateSale_ShouldReturnCreated()
        {
            var sale = new SaleDTO
            {
                SKU = "SKU456",
                OrderType = "Retention",
                OrderNumber = "A001",
                UserId = "u123",
                StoreId = 1,
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(10, 0, 0),
                IsAdditional = false,
                IsDiscounted = false
            };

            var response = await _client.PostAsync("/Sales", ToJsonContent(sale));
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[CreateSale] Status: {response.StatusCode}");
            _output.WriteLine($"[CreateSale] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.Created, content);
        }

        [Fact]
        public async Task CreateSale_ShouldReturnBadRequest_WhenInvalidSku()
        {
            var invalid = new SaleDTO
            {
                SKU = "invalid-sku",
                OrderType = "Acquisition",
                OrderNumber = "B002",
                UserId = "u123",
                StoreId = 1,
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(11, 0, 0),
                IsAdditional = false,
                IsDiscounted = false
            };

            var response = await _client.PostAsync("/Sales", ToJsonContent(invalid));
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[CreateBadSale] Status: {response.StatusCode}");
            _output.WriteLine($"[CreateBadSale] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAllSales_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/Sales");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[GetAllSales] Status: {response.StatusCode}");
            _output.WriteLine($"[GetAllSales] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GetSaleById_ShouldReturnSale()
        {
            var create = new SaleDTO
            {
                SKU = "SKU123",
                OrderType = "Acquisition",
                OrderNumber = "G789",
                UserId = "u123",
                StoreId = 1,
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(12, 0, 0),
                IsAdditional = false,
                IsDiscounted = false
            };

            var created = await _client.PostAsync("/Sales", ToJsonContent(create));
            var newSale = await created.Content.ReadFromJsonAsync<SaleDTO>(_jsonOptions);

            var response = await _client.GetAsync($"/Sales/{newSale!.SaleId}");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[GetSaleById] Status: {response.StatusCode}");
            _output.WriteLine($"[GetSaleById] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("SKU123");
        }

        [Fact]
        public async Task UpdateSale_ShouldReturnNoContent()
        {
            var create = new SaleDTO
            {
                SKU = "SKU123",
                OrderType = "Acquisition",
                OrderNumber = "U543",
                UserId = "u123",
                StoreId = 1,
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(13, 0, 0),
                IsAdditional = false,
                IsDiscounted = false
            };

            var created = await _client.PostAsync("/Sales", ToJsonContent(create));
            var newSale = await created.Content.ReadFromJsonAsync<SaleDTO>(_jsonOptions);
            newSale!.OrderNumber = "U543-Updated";

            var response = await _client.PutAsync($"/Sales/{newSale.SaleId}", ToJsonContent(newSale));
            var result = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[UpdateSale] Status: {response.StatusCode}");
            _output.WriteLine($"[UpdateSale] Body: {result}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteSale_ShouldReturnNoContent()
        {
            var create = new SaleDTO
            {
                SKU = "SKU123",
                OrderType = "Acquisition",
                OrderNumber = "D999",
                UserId = "u123",
                StoreId = 1,
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(14, 0, 0),
                IsAdditional = false,
                IsDiscounted = false
            };

            var created = await _client.PostAsync("/Sales", ToJsonContent(create));
            var newSale = await created.Content.ReadFromJsonAsync<SaleDTO>(_jsonOptions);

            var response = await _client.DeleteAsync($"/Sales/{newSale!.SaleId}");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[DeleteSale] Status: {response.StatusCode}");
            _output.WriteLine($"[DeleteSale] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
