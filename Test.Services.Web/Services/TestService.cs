using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityModel.Client;
using Test.Services.Web.Models;

namespace Test.Services.Web.Services
{
    public class TestService : ITestService
    {
        private readonly HttpClient client;
        private string _accessToken;

        public TestService(HttpClient client)
        {
            this.client = client;
        }

        private async Task<string> GetToken()
        {
            if (!string.IsNullOrWhiteSpace(_accessToken))
            {
                return _accessToken;
            }

            var discoveryDocumentResponse =
                await client.GetDiscoveryDocumentAsync("https://localhost:5010/");
            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }

            var tokenResponse =
                await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest()
                    {
                        Address = discoveryDocumentResponse.TokenEndpoint,
                        ClientId = "test",
                        ClientSecret = "test.secret",
                        Scope = "test1.read test1.write"
                    }
                );
            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            _accessToken = tokenResponse.AccessToken;
            return _accessToken;
        }

        public async Task<IEnumerable<TestDTO>> GetAll()
        {
            client.SetBearerToken(await GetToken());

            var response = await client.GetAsync("WeatherForecast");
            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<TestDTO>>();
            return responseContent ?? Array.Empty<TestDTO>();
        }
    }
}