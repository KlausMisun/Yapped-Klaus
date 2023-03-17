using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yapped.Klaus.Core.Utils;

public abstract class SearchHelperKeywordedFilter<T> {

    public bool IsCaseSensitive { get; set; } = true;
    public string Keyword { get; init; } = string.Empty;

    public abstract Task<IEnumerable<T>> FilterAsync(string[] keywords, IEnumerable<T> values);
}

public class SearchHelper<T> where T : class
{
    public SearchHelper(IEnumerable<SearchHelperKeywordedFilter<T>> filters, string keywordSeparator)
    {
        Filters = filters;
        KeywordSeparator = keywordSeparator;
    }

    public IEnumerable<SearchHelperKeywordedFilter<T>> Filters { get; }
    public string KeywordSeparator { get; }

    public IEnumerable<SearchHelperKeywordedFilter<T>> GetBehaviorsForKeyword(string keyword)
    {
        var results = new List<SearchHelperKeywordedFilter<T>>();

        var filters = Filters.Where(x => x.IsCaseSensitive ?
            x.Keyword == keyword : x.Keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase));

        foreach (var filter in Filters)
        {

            var comparisonType = filter.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if (keyword.Equals(filter.Keyword, comparisonType))
            {
                results.Add(filter);
            }
        }

        return results;
    }

    public Task<IEnumerable<T>> SearchAsync(IEnumerable<T> values, string searchQuery)
    {
        return Task.Run(async () =>
        {
            var keywordList = searchQuery.Split(KeywordSeparator,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (keywordList.Length == 0)
            {
                return values;
            }

            var keyword = keywordList[0];
            var filters = GetBehaviorsForKeyword(string.Empty);

            if (searchQuery.Contains(KeywordSeparator))
            {
                //GlobalMatch for default behavior
                var newBehaviors = GetBehaviorsForKeyword(keyword);

                filters = newBehaviors.Any() ? newBehaviors : Enumerable.Empty<SearchHelperKeywordedFilter<T>>();
            }

            var resultTasks = filters.Select(async filter => await filter.FilterAsync(keywordList, values)).ToList();

            await Task.WhenAll(resultTasks);

            var results = new List<T>();
                
            foreach (var resultTask in resultTasks)
            {
                results.AddRange(await resultTask);
            }

            return results.Distinct();
        });
    }

}
