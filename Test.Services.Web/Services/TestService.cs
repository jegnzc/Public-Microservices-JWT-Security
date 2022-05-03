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

        public TestService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<TestDTO>> GetAll()
        {
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