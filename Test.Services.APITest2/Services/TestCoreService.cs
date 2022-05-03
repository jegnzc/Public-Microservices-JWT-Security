using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Test.Services.Web.Models;

namespace Test.Services.Web.Services
{
    public class TestCoreService : ITestCoreService
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _accessToken;

        public TestCoreService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<string> GetToken()
        {
            if (!string.IsNullOrWhiteSpace(_accessToken))
            {
                return _accessToken;
            }

            var discoveryDocumentResponse = await client
                .GetDiscoveryDocumentAsync("https://localhost:5010/");
            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }

            var test = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var customParams = new Dictionary<string, string>
            {
                { "subject_token_type", "urn:ietf:params:oauth:token-type:access_token"},
                { "subject_token", test},
                { "scope", "openid profile testcore.fullaccess" }
            };

            var customP = new Parameters(customParams);
            var tokenResponse = await client.RequestTokenAsync(new TokenRequest()
            {
                Address = discoveryDocumentResponse.TokenEndpoint,
                GrantType = "urn:ietf:params:oauth:grant-type:token-exchange",
                Parameters = customP,
                ClientId = "test2todownstreamtokenexchangeclient",
                ClientSecret = "test.secret"
            });

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
            var response = await client.GetAsync("weatherforecast");
            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<TestDTO>>();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return responseContent;
        }
    }
}