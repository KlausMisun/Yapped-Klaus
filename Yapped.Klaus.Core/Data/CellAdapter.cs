using FSParam;
using Yapped.Klaus.WPF.Converters;

namespace Yapped.Klaus.Core.Data;

public class CellAdapter
{
    public CellAdapter(Param.Cell cell)
    {
        ParamCell = cell;
    }

    public Param.Cell ParamCell { get; private set; }

    public object Value
    {
        get
        {
            return ParamCell.Value;
        }
        set
        {
            var newValue = StringToCellTypeConverter.ConvertBack(value, ParamCell.Def);
            ParamCell.SetValue(newValue);
        }
    }
}

