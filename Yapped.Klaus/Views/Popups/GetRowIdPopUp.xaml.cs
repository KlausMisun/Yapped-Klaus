using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Yapped.Klaus.WPF.Base;
using Yapped.Klaus.WPF.ViewModels.Popups;

namespace Yapped.Klaus.WPF.Views.Popups
{
    /// <summary>
    /// Interaction logic for GetRowIdPopUp.xaml
    /// </summary>
    public partial class GetRowIdPopUp : BasePopup
    {
        private readonly GetRowIdPopupModel vm;

        public GetRowIdPopUp(GetRowIdPopupModel vm)
        {
            this.DataContext = vm;
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            this.vm = vm;
        }

        private void Goto_Click(object sender, RoutedEventArgs e)
        {
            vm.Goto();
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            vm.Cancel();
            this.Close();
        }
    }
}
