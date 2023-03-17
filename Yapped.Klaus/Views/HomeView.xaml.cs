using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yapped.Klaus.Base.ViewModel;
using Yapped.Klaus.WPF.Base;
using Yapped.Klaus.WPF.Views.Popups;

namespace Yapped.Klaus.WPF.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomeView : BaseView
    {
        public HomeView(BaseViewModel vm)
        {
            this.DataContext = vm;
            Focusable = true;
            InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (sender is not DataGrid grid)
            {
                return;
            }

            grid.ScrollIntoView(grid.SelectedItem);
        }
    }
}
