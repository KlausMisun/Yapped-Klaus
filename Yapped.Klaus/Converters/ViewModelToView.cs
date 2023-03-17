using System;
using System.Globalization;
using System.Windows.Data;
using Yapped.Klaus.Base.ViewModel;
using Yapped.Klaus.WPF.Base;

namespace Yapped.Klaus.WPF.Converters;

public class ViewModelToView : IValueConverter
{
    public static readonly ViewModelToView Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not BaseViewModel vm)
        {
            throw new Exception("Bad Input");
        }

        var vmType = vm.GetType();
        var viewName = vmType.FullName!.Replace("ViewModel", "View");
        var viewType = Type.GetType(viewName)
            ?? throw new Exception("Bad Input");

        BaseView? view = Activator.CreateInstance(viewType, new object[] { vm }) as BaseView
            ?? throw new Exception("View Couldn't be made");

        view.DataContext = vm;

        return view;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
