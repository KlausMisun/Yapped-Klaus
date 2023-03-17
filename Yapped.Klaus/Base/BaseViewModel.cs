using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Yapped.Klaus.Base.ViewModel;

public class BaseViewModel : ObservableObject
{
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}
