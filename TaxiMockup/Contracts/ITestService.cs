using Microsoft.ServiceFabric.Services.Remoting;

namespace Contracts
{
    public interface ITestService :IService
    {

        Task<string> HelloWorld();

    }
}
