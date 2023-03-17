using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Yapped.Klaus.Base.ViewModel;
using Yapped.Klaus.Core.Data;
using Yapped.Klaus.Core.DataHelper;
using Yapped.Klaus.WPF.ViewModelUtils;
using Yapped.Klaus.WPF.ViewUtils;

namespace Yapped.Klaus.WPF;

public static class Helpers
{
    public static void RegisterServices(AppConfiguration appConfiguration)
    {
        ServiceCollection services = new();

        // Register Services Here

        // Get all ViewModels
        var viewModels = typeof(App).Assembly.GetTypes()
        .Where(t => t.IsSubclassOf(typeof(BaseViewModel)))
        ?? throw new Exception("Huge Error");

        // Register all ViewModels as Transient
        foreach (var viewModel in viewModels)
        {
            services.AddTransient(viewModel);
        }

        services.AddSingleton(appConfiguration);
        services.AddSingleton<DataManagers>();
        services.AddSingleton<IViewModelUILink, ViewModelUILink>();
        // Register Services Here


        var provider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(provider);

    }
}
