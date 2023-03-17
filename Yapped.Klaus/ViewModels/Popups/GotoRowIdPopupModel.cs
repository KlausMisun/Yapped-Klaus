using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yapped.Klaus.Base.ViewModel;

namespace Yapped.Klaus.WPF.ViewModels.Popups;

public partial class GetRowIdPopupModel : BaseViewModel
{
    [ObservableProperty] private int _id = 0;
    [ObservableProperty] private bool _confirmed = false;

    [RelayCommand]
    public void Goto()
    {
        Confirmed = true;
    }

    [RelayCommand]
    public void Cancel() {
        Confirmed = false;
    }
}
