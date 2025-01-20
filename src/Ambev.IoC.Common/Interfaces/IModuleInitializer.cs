using Microsoft.Extensions.DependencyInjection;

namespace Ambev.IoC.Common.Interfaces
{
    public interface IModuleInitializer
    {
        void Initialize(IServiceCollection services);
    }
}
