using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yapped.Klaus.Core.Data;
using Yapped.Klaus.Core.DataHelper;

namespace Yapped.Klaus.Core.Utils.SearchUtils;

public class GeneralParamFilter : SearchHelperKeywordedFilter<PARAMAdapter>
{
    public override Task<IEnumerable<PARAMAdapter>> FilterAsync(string[] keywords, IEnumerable<PARAMAdapter> values)
    {
        var targetKeyword = keywords.Last();

        return Task.Run(() => values.Where(x => x.ParamName
        .Contains(keywords.Last(), StringComparison.OrdinalIgnoreCase)));
    }
}

public class ExactParamFilter : SearchHelperKeywordedFilter<PARAMAdapter>
{
    public ExactParamFilter()
    {
        Keyword = "exact";
    }

    public override Task<IEnumerable<PARAMAdapter>> FilterAsync(string[] keywords, IEnumerable<PARAMAdapter> values)
    {
        var targetKeyword = keywords.Last();

        return Task.Run(() => values.Where(x => x.ParamName == targetKeyword));
    }
}
