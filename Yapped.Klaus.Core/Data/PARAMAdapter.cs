using FSParam;
using SoulsFormats;
using System.Diagnostics;
using System.Globalization;

namespace Yapped.Klaus.Core.Data;

public class PARAMAdapter
{
    public PARAMAdapter(Dictionary<int, string> rowNames, 
        Param param, string paramName)
    {
        Param = param;
        ParamName = paramName;

        var lazyRows = new List<RowAdapter>();

        foreach (var row in Param.Rows)
        {

            if (!rowNames.TryGetValue(row.ID, out string? name))
            {
                name = row.Name;
            }

            lazyRows.Add(new RowAdapter(name, row));
        }

        Rows = lazyRows;
    }

    public Param Param { get; init; }
    public string ParamName { get; }
    public List<RowAdapter> Rows { get; init; }
}

