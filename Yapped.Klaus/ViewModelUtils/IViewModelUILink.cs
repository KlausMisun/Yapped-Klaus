using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yapped.Klaus.WPF.ViewModelUtils;

// In the case in a far far future, I decide that I want to split the UI from the ViewModels and use Avalonia or some other tooling
public interface IViewModelUILink
{
    public string? GetFilePathFromPopup(string title, string fileName);
    public int? GetGotoRowID();
    public Task InfoPopupAsync(string title, string message);
    public Task ErrorPopupAsync();
}
