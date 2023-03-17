using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using Yapped.Klaus.Base.ViewModel;
using Yapped.Klaus.Core.Data;
using Yapped.Klaus.Core.DataHelper;
using Yapped.Klaus.Core.Utils;
using Yapped.Klaus.Core.Utils.SearchUtils;
using Yapped.Klaus.WPF.ViewModelUtils;

namespace Yapped.Klaus.WPF.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly AppConfiguration _appConfiguration;
    private readonly IViewModelUILink _link;

    // Param information relating to filtering and displating the parameter names
    [ObservableProperty] private string _paramFilterText = string.Empty;
    [ObservableProperty] private List<PARAMAdapter> _data = new();
    [ObservableProperty]
    private ObservableCollection<PARAMAdapter> _filteredData =
        new();


    // parameter rows shown after the selection of a parameter
    [ObservableProperty] private string _rowFilterText = string.Empty;
    [ObservableProperty] private PARAMAdapter? _selectedParam;
    [ObservableProperty]
    private ObservableCollection<RowAdapter> _filteredRows =
        new();

    // the filtering and listing of cells to be displayed after the selection of a row
    [ObservableProperty] private string _cellsFilterText = string.Empty;
    [ObservableProperty] private RowAdapter? _selectedRow;
    [ObservableProperty]
    private ObservableCollection<CellAdapter> _filteredSelectedRowCells =
        new();

    [ObservableProperty] private int _selectedCellIndex = 0;

    [ObservableProperty] private bool _showFieldType = false;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowDisplayName))]
    private bool _showInternalName = false;
    public bool ShowDisplayName => !ShowInternalName;

    [ObservableProperty] private bool _showFilters;

    // Searcher Helpers inspired by DSMapStudio
    private readonly SearchHelper<PARAMAdapter> _paramSearcher;
    private readonly SearchHelper<RowAdapter> _rowSearcher;
    private readonly SearchHelper<CellAdapter> _cellSearcher;

    public HomeViewModel(AppConfiguration appConfiguration, IViewModelUILink link)
    {
        _data = new();
        _appConfiguration = appConfiguration;
        this._link = link;
        ShowInternalName = appConfiguration.ShowInternalCellNames;
        ShowFieldType = appConfiguration.ShowFieldDataType;
        ShowFilters = appConfiguration.ShowFilters;

        //Creating behavior exact and standard search | seek to implement fuzzy search later
        var paramFilters = new List<SearchHelperKeywordedFilter<PARAMAdapter>>()
        {
            new GeneralParamFilter(),
            new ExactParamFilter(),
        };

        _paramSearcher = new(paramFilters, ":");

        var rowFilters = new List<SearchHelperKeywordedFilter<RowAdapter>>()
        {
            new GeneralIDRowFilter(),
            new GeneralNameRowFilter(),
            new ExactIDRowFilter(),
            new ExactNameRowFilter(),
            new FieldCompareRowFilter(),
            new FieldEqualRowFilter(),
        };

        _rowSearcher = new(rowFilters, ":");

        // behaviors to search for rows with special searches for the keys

        var cellFilters = new List<SearchHelperKeywordedFilter<CellAdapter>>()
        {
            new GeneralCellNameFilter(),
            new ExactCellNameFilter(),
            new GeneralCellValueFilter(),
            new ExactCellValueFilter(),
        };

        _cellSearcher = new(cellFilters, ":");
    }

    public override async Task InitializeAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        Data = (await DataManagers.LoadBNDParamsAsync(_appConfiguration.GameParamPath)).ToList();
    }

    #region Filtering_Methods

    async Task FilterParams()
    {
        FilteredData = new((await _paramSearcher.SearchAsync(Data, ParamFilterText))
            .OrderBy(x => x.ParamName));

        SelectedParam ??= FilteredData.FirstOrDefault();
    }

    async Task FilterRows()
    {
        if (SelectedParam is null) return;

        var searchResults = (await _rowSearcher.SearchAsync(SelectedParam.Rows, RowFilterText)).OrderBy(x => x.ID);
        FilteredRows = new ObservableCollection<RowAdapter>(searchResults);

        SelectedRow ??= searchResults.FirstOrDefault();
    }

    async Task FilterCells()
    {
        if (SelectedRow is null) return;
        var cellsToSearch = SelectedRow.Cells
            .Where(x => x.ParamCell.Def.DisplayType != PARAMDEF.DefType.dummy8);

        FilteredSelectedRowCells =
            new((await _cellSearcher.SearchAsync(cellsToSearch, CellsFilterText))
            .OrderBy(x => x.ParamCell.Def.DisplayName));
    }

    #endregion

    #region Reacting_To_FrontEnd_Data_Changes

    async partial void OnParamFilterTextChanged(string value)
    {
        await FilterParams();
    }

    async partial void OnRowFilterTextChanged(string value)
    {
        await FilterRows();
    }

    async partial void OnCellsFilterTextChanged(string value)
    {
        await FilterCells();
    }

    async partial void OnDataChanged(List<PARAMAdapter> value)
    {
        await FilterParams();

        SelectedParam = FilteredData.FirstOrDefault();
    }

    async partial void OnSelectedParamChanged(PARAMAdapter? value)
    {
        if (value is null)
        {
            FilteredRows.Clear();
        }

        await FilterRows();

        SelectedRow = FilteredRows.FirstOrDefault();
    }

    async partial void OnSelectedRowChanged(RowAdapter? value)
    {
        if (value is null)
        {
            FilteredSelectedRowCells.Clear();
        }

        await FilterCells();

        if (FilteredSelectedRowCells.Count > 0)
        {
            SelectedCellIndex = 0;
        }
    }

    #endregion

    #region Commands_For_Clicks_On_UI

    [RelayCommand]
    public async Task OpenFile()
    {
        var filePath = _link.GetFilePathFromPopup(
            "Select gameparam.parambnd.dcx for Sekiro",
            "gameparam.parambnd.dcx"
            );

        if (filePath is not null)
        {
            _appConfiguration.GameParamPath = filePath;
            await LoadData();
        }
    }

    [RelayCommand]
    public async Task SaveFile()
    {
        await Task.Run(async () =>
        {
            await DataManagers.SaveBNDAync(Data, _appConfiguration.GameParamPath);
            MessageBox.Show($"Finished Saving at {_appConfiguration.GameParamPath}", "Saving Progress", MessageBoxButton.OK, MessageBoxImage.Information);
        });

        RestoreBackupCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public async Task ReloadCurrentSave()
    {
        Data = (await DataManagers.LoadBNDParamsAsync(_appConfiguration.GameParamPath)).ToList();
    }

    [RelayCommand(CanExecute = nameof(CanRestoreBackup))]
    public async Task RestoreBackup()
    {
        Data = (await DataManagers.LoadBNDParamsAsync(_appConfiguration.BackupGameParamPath)).ToList();
    }

    private bool CanRestoreBackup()
    {
        return File.Exists(_appConfiguration.BackupGameParamPath);
    }

    [RelayCommand]
    public Task ShowParamFile()
    {
        try
        {
            Process.Start("explorer.exe", Path.GetDirectoryName(_appConfiguration.GameParamPath)!);
        }
        catch
        {
            SystemSounds.Hand.Play();
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task CreateBackUpFile()
    {
        var task = Task.Run(() => DataManagers.CreateBNDBackup(_appConfiguration.GameParamPath, _appConfiguration.BackupGameParamPath));

        RestoreBackupCommand.NotifyCanExecuteChanged();

        return task;
    }

    [RelayCommand]
    public async Task SaveAndBackUpFile()
    {
        await SaveFile();
        await CreateBackUpFile();
        RestoreBackupCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public Task ToggleFieldType()
    {
        _appConfiguration.ShowFieldType = !_appConfiguration.ShowFieldType;
        ShowFieldType = _appConfiguration.ShowFieldType;

        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task ToggleFieldName()
    {
        _appConfiguration.ShowInternalCellNames = !_appConfiguration.ShowInternalCellNames;
        ShowInternalName = _appConfiguration.ShowInternalCellNames;

        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task ToggleFilterVisibility()
    {
        _appConfiguration.ShowFilters = !_appConfiguration.ShowFilters;
        ShowFilters = _appConfiguration.ShowFilters;

        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task GotoRow() {
    
        var rowId = _link.GetGotoRowID();

        if (rowId is null)
        {
            return Task.CompletedTask;
        }

        var row = FilteredRows.FirstOrDefault(x => x.ID == rowId);

        if (row is not null)
        {
            SelectedRow = row;
            _link.InfoPopupAsync("Goto Row", "Row Found!");
        }

        return Task.CompletedTask;
    }

    #endregion
}
