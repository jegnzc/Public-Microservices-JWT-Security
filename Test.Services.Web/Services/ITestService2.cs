using Test.Services.Web.Models;

namespace Test.Services.APITest2.Services
{
    public interface ITestService2
    {
        public Task<IEnumerable<TestDTO>> GetAll();
    }
}