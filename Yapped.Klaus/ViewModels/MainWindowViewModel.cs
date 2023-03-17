using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;
using Yapped.Klaus.Base.ViewModel;
using Yapped.Klaus.Core.Data;

namespace Yapped.Klaus.WPF.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    public MainWindowViewModel()
    {
        var config = Ioc.Default.GetService<AppConfiguration>()!;
        
        if (config.SuccessfulLoad)
        {
            _currentPageVM = Ioc.Default.GetService<HomeViewModel>()!;
        }
        else
        {
            Application.Current.Shutdown();
        }
    }

    [ObservableProperty] private BaseViewModel? _currentPageVM;
}
