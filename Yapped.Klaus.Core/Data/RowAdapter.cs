using FSParam;
using static SoulsFormats.PARAM;

namespace Yapped.Klaus.Core.Data;

public class RowAdapter
{
    private string? _customName;

    public RowAdapter(string? name, Param.Row row)
    {
        _customName = name;
        Row = row;
        Cells = row.CellHandles.Select(c => new CellAdapter(c)).ToList();
    }

    public string? Text
    {
        get
        {
            return _customName ?? Row.Name;
        }

        set => _customName = value;
    }

    public int ID => Row.ID;

    public List<CellAdapter> Cells { get; set; }

    public Param.Row Row { get; }
}

