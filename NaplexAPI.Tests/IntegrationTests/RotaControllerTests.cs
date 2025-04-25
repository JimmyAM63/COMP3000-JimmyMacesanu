using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTests.Setup;
using Microsoft.Extensions.DependencyInjection;
using NaplexAPI.Infrastructure;
using NaplexAPI.Models.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class RotaControllerTests
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

        public RotaControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateRota_ReturnsCreated_WhenDifferentDate()
        {
            var rota = new RotaDTO
            {
                UserId = "u123",
                StoreId = 1,
                Date = new DateTime(2025, 5, 5),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                IsLeave = false,
                IsOff = false
            };

            var response = await _client.PostAsync("/Rota", ToJsonContent(rota));
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[Create] Status: {response.StatusCode}");
            _output.WriteLine($"[Create] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.Created, content);
        }

        [Fact]
        public async Task CreateRota_ReturnsBadRequest_WhenSameDateExists()
        {
            var rota = new RotaDTO
            {
                UserId = "u123",
                StoreId = 1,
                Date = new DateTime(2025, 5, 1),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                IsLeave = false,
                IsOff = false
            };

            var response = await _client.PostAsync("/Rota", ToJsonContent(rota));
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[DupCreate] Status: {response.StatusCode}");
            _output.WriteLine($"[DupCreate] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest, content);
        }

        [Fact]
        public async Task GetRotaById_ShouldReturnRota()
        {
            var get = await _client.GetAsync("/Rota/user/u123");
            var list = await get.Content.ReadFromJsonAsync<List<RotaDTO>>();
            var rotaId = list![0].RotaId;

            var response = await _client.GetAsync($"/Rota/{rotaId}");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[GetById] Status: {response.StatusCode}");
            _output.WriteLine($"[GetById] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("u123");
        }

        [Fact]
        public async Task GetRotasForStore_ShouldReturnSeededRota()
        {
            var response = await _client.GetAsync("/Rota/store/1");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[GetForStore] Status: {response.StatusCode}");
            _output.WriteLine($"[GetForStore] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("u123");
        }

        [Fact]
        public async Task GetWeeklyRotasForStore_ShouldReturnSeededRota()
        {
            var response = await _client.GetAsync("/Rota/store/1/weekly?start=2025-05-01&end=2025-05-07");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[GetWeekly] Status: {response.StatusCode}");
            _output.WriteLine($"[GetWeekly] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRotasForUser_ShouldReturnSeededRota()
        {
            var response = await _client.GetAsync("/Rota/user/u123");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[GetRotasForUser] Status: {response.StatusCode}");
            _output.WriteLine($"[GetRotasForUser] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateRota_ShouldUpdateSeededRotaSuccessfully()
        {
            var get = await _client.GetAsync("/Rota/user/u456");
            var list = await get.Content.ReadFromJsonAsync<List<RotaDTO>>();

            list.Should().NotBeNull().And.HaveCountGreaterThan(0);
            var seeded = list![0];
            seeded.StartTime = new TimeSpan(14, 0, 0);

            var response = await _client.PutAsync($"/Rota/{seeded.RotaId}", ToJsonContent(seeded));
            var result = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[Update] Status: {response.StatusCode}");
            _output.WriteLine($"[Update] Body: {result}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteRota_ShouldDeleteSeededRota()
        {
            var get = await _client.GetAsync("/Rota/user/u456");
            var rotas = await get.Content.ReadFromJsonAsync<List<RotaDTO>>();

            rotas.Should().NotBeNull().And.HaveCountGreaterThan(0);
            var rotaId = rotas[0].RotaId;

            var response = await _client.DeleteAsync($"/Rota/{rotaId}");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"[Delete] Status: {response.StatusCode}");
            _output.WriteLine($"[Delete] Body: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }

    public class JsonStringDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _dateFormat = "yyyy-MM-dd";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            DateTime.ParseExact(reader.GetString(), _dateFormat, System.Globalization.CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString(_dateFormat));
    }
}
