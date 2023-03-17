using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;
using Yapped.Klaus.WPF.ViewModels;

namespace Yapped.Klaus.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainWindowViewModel>()!;
        }
    }
}
