using System.Text.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Test.Services.Web.Models;

namespace Test.Services.APITest2.Services
{
    public class TestService2 : ITestService2
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TestService2(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<TestDTO>> GetAll()
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.SetBearerToken(token);

            var response = await client.GetAsync("weatherforecast");
            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<TestDTO>>();
            return responseContent ?? Array.Empty<TestDTO>();
        }
    }
}