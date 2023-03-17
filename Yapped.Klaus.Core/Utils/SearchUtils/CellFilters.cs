using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yapped.Klaus.Core.Data;
using Yapped.Klaus.WPF.Converters;

namespace Yapped.Klaus.Core.Utils.SearchUtils;

public class GeneralCellNameFilter : SearchHelperKeywordedFilter<CellAdapter>
{
    public override Task<IEnumerable<CellAdapter>> FilterAsync(string[] keywords, IEnumerable<CellAdapter> values)
    {
        var targetKeyword = keywords.Last();

        return Task.Run(() => values.Where(x => x.ParamCell.Def.CellNameContains(targetKeyword)));
    }
}

public class GeneralCellValueFilter : SearchHelperKeywordedFilter<CellAdapter>
{
    public override Task<IEnumerable<CellAdapter>> FilterAsync(string[] keywords, IEnumerable<CellAdapter> values)
    {
        var targetKeyword = keywords.Last();
        return Task.Run(() => values.Where(x => x.Value.ToString()?.Contains(targetKeyword) ?? false));
    }
}

public class ExactCellNameFilter : SearchHelperKeywordedFilter<CellAdapter>
{
    public override Task<IEnumerable<CellAdapter>> FilterAsync(string[] keywords, IEnumerable<CellAdapter> values)
    {
        var targetKeyword = keywords.Last();
        return Task.Run(() => values.Where(x => x.ParamCell.Def.CellNameEquals(targetKeyword)));
    }
}

public class ExactCellValueFilter : SearchHelperKeywordedFilter<CellAdapter>
{
    public override Task<IEnumerable<CellAdapter>> FilterAsync(string[] keywords, IEnumerable<CellAdapter> values)
    {
        var targetValue = keywords.Last();

        return Task.Run(() => values.Where(cell =>
        {
            if (!StringToCellTypeConverter.CanConvertToParamDefType(targetValue, cell.ParamCell.Def.DisplayType))
            {
                return false;
            }

            var value = StringToCellTypeConverter.ConvertBack(targetValue, cell.ParamCell.Def);

            if (cell.Value.Equals(value))
            {
                return true;
            }

            return false;
        }));
    }
}



