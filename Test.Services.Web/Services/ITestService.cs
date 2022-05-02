using Test.Services.Web.Models;

namespace Test.Services.Web.Services
{
    public interface ITestService
    {
        public Task<IEnumerable<TestDTO>> GetAll();
    }
}