using System.Text.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Test.Services.Web.Models;

namespace Test.Services.APITest2.Services
{
    public class TestService2 : ITestService2
    {
        private readonly HttpClient client;

        public TestService2(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<TestDTO>> GetAll()
        {
            var response = await client.GetAsync("weatherforecast");
            var responseContent = await response.Content.ReadFromJsonAsync<IEnumerable<TestDTO>>();
            return responseContent ?? Array.Empty<TestDTO>();
        }
    }
}