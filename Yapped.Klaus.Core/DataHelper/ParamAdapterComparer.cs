using Yapped.Klaus.Core.Data;

namespace Yapped.Klaus.Core.DataHelper;

public class ParamAdapterComparer : IComparer<PARAMAdapter>
{
    public int Compare(PARAMAdapter? x, PARAMAdapter? y)
    {
        return (x, y) switch
        {
            (null, null) => 0,
            (null, _) => 1,
            (_, null) => -1,
            (_, _) => x.ParamName.CompareTo(y.ParamName)
        };
    }
}

