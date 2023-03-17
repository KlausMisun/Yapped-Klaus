using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yapped.Klaus.WPF.ViewModels.Popups;
using Yapped.Klaus.WPF.ViewModelUtils;
using Yapped.Klaus.WPF.Views.Popups;

namespace Yapped.Klaus.WPF.ViewUtils;

public class ViewModelUILink : IViewModelUILink
{
    public Task ErrorPopupAsync()
    {
        MessageBox.Show("HELLO");
        return Task.CompletedTask;
    }

    public string? GetFilePathFromPopup(string title, string fileName)
    {
        OpenFileDialog fileDialog = new()
        {
            FileName = fileName,
            CheckFileExists = true,
            Title = title
        };

        var result = fileDialog.ShowDialog() ?? false;

        return result ? fileDialog.FileName : null;
    }

    public int? GetGotoRowID()
    {
        var vm = Ioc.Default.GetService<GetRowIdPopupModel>();

        if (vm == null) return null;

        GetRowIdPopUp view = new(vm);

        view.ShowDialog();

        return vm.Confirmed ? vm.Id : null;
    }

    public Task InfoPopupAsync(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        return Task.CompletedTask;
    }
}
