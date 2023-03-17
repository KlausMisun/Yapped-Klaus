using System;
using System.Windows.Controls;
using Yapped.Klaus.Base.ViewModel;

namespace Yapped.Klaus.WPF.Base;

/// <summary>
/// The base type for MVVM Views
/// </summary>
/// <typeparam name="T">The ViewModel Name</typeparam>
public class BaseView : UserControl
{
    public BaseViewModel? VM => this.DataContext as BaseViewModel;

    public BaseView() : base()
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
