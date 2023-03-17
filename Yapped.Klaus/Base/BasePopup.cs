using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yapped.Klaus.Base.ViewModel;

namespace Yapped.Klaus.WPF.Base;

public class BasePopup : Window
{
    public BaseViewModel? VM => this.DataContext as BaseViewModel;

    public BasePopup() : base()
    {
        this.FocusVisualStyle = null;
    }

    protected override async void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        if (VM is null)
        {
            return;
        }


        await VM.InitializeAsync();


    }
}
