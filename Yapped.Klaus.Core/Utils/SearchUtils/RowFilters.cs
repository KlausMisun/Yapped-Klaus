using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yapped.Klaus.Core.Data;
using Yapped.Klaus.WPF.Converters;
using static SoulsFormats.GRASS;

namespace Yapped.Klaus.Core.Utils.SearchUtils;

public class GeneralNameRowFilter : SearchHelperKeywordedFilter<RowAdapter>
{
    public override Task<IEnumerable<RowAdapter>> FilterAsync(string[] keywords, IEnumerable<RowAdapter> values)
    {
        var targetKeyWord = keywords.Last();

        return Task.Run(() => values.Where(row => row.Text?.Contains(targetKeyWord, StringComparison.OrdinalIgnoreCase) ?? false));
    }
}

public class GeneralIDRowFilter : SearchHelperKeywordedFilter<RowAdapter>
{
    public override Task<IEnumerable<RowAdapter>> FilterAsync(string[] keywords, IEnumerable<RowAdapter> values)
    {
        var targetKeyWord = keywords.Last();

        return Task.Run(() => values.Where(row => row.ID.ToString().Contains(targetKeyWord, StringComparison.OrdinalIgnoreCase)));
    }
}

public class ExactNameRowFilter : SearchHelperKeywordedFilter<RowAdapter>
{
    public ExactNameRowFilter()
    {
        Keyword = "exact";
    }

    public override Task<IEnumerable<RowAdapter>> FilterAsync(string[] keywords, IEnumerable<RowAdapter> values)
    {
        var targetKeyWord = keywords.Last();

        return Task.Run(() => values.Where(row => row.Text == targetKeyWord));
    }
}

public class ExactIDRowFilter : SearchHelperKeywordedFilter<RowAdapter>
{
    public ExactIDRowFilter()
    {
        Keyword = "exact";
    }

    public override Task<IEnumerable<RowAdapter>> FilterAsync(string[] keywords, IEnumerable<RowAdapter> values)
    {
        var targetKeyword = keywords.Last();

        if (!int.TryParse(targetKeyword, out int Id))
        {
            return Task.FromResult(Enumerable.Empty<RowAdapter>());
        }

        return Task.Run(() => values.Where(row => row.ID == Id));
    }
}

public class FieldEqualRowFilter : SearchHelperKeywordedFilter<RowAdapter>
{
    public FieldEqualRowFilter()
    {
        Keyword = "field";
    }

    public override async Task<IEnumerable<RowAdapter>> FilterAsync(string[] keywords, IEnumerable<RowAdapter> values)
    {
        if (keywords.Length <= 2)
        {
            return values;
        }

        var field = keywords[1];
        var valueString = keywords[2];


        var cell = values.FirstOrDefault();

        //check field exists
        if (cell is null || 
            !cell.Cells.Any(x => x.ParamCell.Def.CellNameContains(field)))
        {
            return Enumerable.Empty<RowAdapter>();
        }

        var bag = new ConcurrentBag<RowAdapter>();
        var taskLists = new List<Task>();

        foreach (var row in values)
        {
            var task = Task.Run(() =>
            {
                var cellsToCheck = row.Cells
                    .Where(x => x.ParamCell.Def.CellNameContains(field));

                foreach (var cell in cellsToCheck)
                {
                    if (!StringToCellTypeConverter.CanConvertToParamDefType(valueString, cell.ParamCell.Def.DisplayType))
                    {
                        continue;
                    }

                    var value = StringToCellTypeConverter.ConvertBack(valueString, cell.ParamCell.Def);

                    if (cell.ParamCell.Value.Equals(value))
                    {
                        bag.Add(row);
                        break;
                    }
                }
            });

            taskLists.Add(task);
        }

        await Task.WhenAll(taskLists);
        return bag;
    }
}
public class FieldCompareRowFilter : SearchHelperKeywordedFilter<RowAdapter>
{
    public FieldCompareRowFilter()
    {
        Keyword = "field";
    }

    public string[] Operators { get; } = new[] { ">", ">=", "<", "<=" };

    public override async Task<IEnumerable<RowAdapter>> FilterAsync(string[] keywords, IEnumerable<RowAdapter> values)
    {
        if (keywords.Length <= 2)
        {
            return values;
        }

        var field = keywords[1];
        var valueString = keywords[2];

        //check field exists
        var cell = values.FirstOrDefault();

        //check field exists
        if (cell is null ||
            !cell.Cells.Any(x => x.ParamCell.Def.CellNameContains(field)))
        {
            return Enumerable.Empty<RowAdapter>();
        }

        var valueStrings = keywords[2].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (!Operators.Any(x => valueStrings.Contains(x))
            || valueStrings.Length != 2)
        {
            return Enumerable.Empty<RowAdapter>();
        }

        var valueToCompare = valueStrings.Last();
        var operation = valueStrings.First() switch
        {
            ">" => ComparisonOperation.GreaterThan,
            ">=" => ComparisonOperation.GreatherThanOrEqual,
            "<" => ComparisonOperation.SmallerThan,
            "<=" => ComparisonOperation.SmallerThanOrEqual,
            _ => ComparisonOperation.None
        };

        if (operation == ComparisonOperation.None)
        {
            return Enumerable.Empty<RowAdapter>();
        }

        var bag = new ConcurrentBag<RowAdapter>();
        var taskLists = new List<Task>();

        foreach (var row in values)
        {
            var task = Task.Run(() =>
            {
                var cellsToCheck = row.Cells
                    .Where(x => x.ParamCell.Def.CellNameContains(field));

                foreach (var cell in cellsToCheck)
                {
                    if (!StringToCellTypeConverter.CanConvertToParamDefType(valueToCompare, cell.ParamCell.Def.DisplayType))
                    {
                        continue;
                    }

                    var value = StringToCellTypeConverter.ConvertBack(valueToCompare, cell.ParamCell.Def);

                    if (cell.Value is not IComparable cellValue) continue;

                    switch (operation)
                    {
                        case ComparisonOperation.GreaterThan:
                            if (cellValue.CompareTo(value) > 0)
                            {
                                bag.Add(row);
                                return;
                            }
                            break;
                        case ComparisonOperation.GreatherThanOrEqual:
                            if (cellValue.CompareTo(value) >= 0)
                            {
                                bag.Add(row);
                                return;
                            }
                            break;
                        case ComparisonOperation.SmallerThan:
                            if (cellValue.CompareTo(value) < 0)
                            {
                                bag.Add(row);
                                return;
                            }
                            break;
                        case ComparisonOperation.SmallerThanOrEqual:
                            if (cellValue.CompareTo(value) <= 0)
                            {
                                bag.Add(row);
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }
            });

            taskLists.Add(task);
        }

        await Task.WhenAll(taskLists);

        return bag;
    }
}
