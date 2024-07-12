using Microsoft.Extensions.DependencyInjection;

namespace CSharpLocalAndRemote.Di;

public static class AppInjector
{
    private static IServiceProvider _serviceProvider;

    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public static T GetService<T>() where T : class => _serviceProvider.GetService<T>();
   
}