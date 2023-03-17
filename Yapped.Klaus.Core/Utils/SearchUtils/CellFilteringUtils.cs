using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yapped.Klaus.Core.Utils.SearchUtils;

public static class CellFilteringUtils
{
    public static bool CellNameContains(this PARAMDEF.Field field, string name)
    {
        return field.InternalName.Contains(name, StringComparison.OrdinalIgnoreCase) 
            || field.DisplayName.Contains(name, StringComparison.OrdinalIgnoreCase);
    }

    public static bool CellNameEquals(this PARAMDEF.Field field, string name)
    {
        return field.InternalName.Equals(name) || field.DisplayName.Equals(name);
    }
}
