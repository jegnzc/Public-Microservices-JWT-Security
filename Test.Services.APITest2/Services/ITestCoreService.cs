using Test.Services.Web.Models;

namespace Test.Services.Web.Services
{
    public interface ITestCoreService
    {
        public Task<IEnumerable<TestDTO>> GetAll();
    }
}