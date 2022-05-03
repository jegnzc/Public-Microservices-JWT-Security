using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Test.Services.Web.Models;

namespace Test.Services.Web.Services
{
    public class TestService : ITestService
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TestService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<TestDTO>> GetAll()
        {
            var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.SetBearerToken(token);
            var response = await client.GetAsync("weatherforecast");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var test = await response.Content.ReadFromJsonAsync<IEnumerable<TestDTO>>();
            return test;
        }
    }
}