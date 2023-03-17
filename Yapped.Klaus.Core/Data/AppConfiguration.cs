namespace Yapped.Klaus.Core.Data;

public class AppConfiguration
{
    public AppConfiguration(string gameParamPath)
    {
        if (ValidatePath(gameParamPath))
        {
            this.gameParamPath = gameParamPath;
            return;
        }

        throw new Exception("Bad File Path");
    }

    public const string AssetFolderPath = @".\Assets\Paramdex\SDT";
    public static string ParamDefsFolderPath => Path.Combine(AssetFolderPath, @"Defs");
    public static string ParamNamesFolderPath => Path.Combine(AssetFolderPath, @"Names");
    public static string ParamNameOverridesFolderPath => Path.Combine(AssetFolderPath, @"NameOverrides");
    public static string LocalAppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Yapped.Klaus");

    public const string AppConfigSaveName = "AppConfig.json";
    private string gameParamPath;

    public static string AppConfigSavePath => Path.Combine(LocalAppDataPath, AppConfigSaveName);

    public static bool ValidatePath(string value)
    {
        return (value.EndsWith(".parambnd.dcx") && File.Exists(value));
    }


    public string GameParamPath { get => gameParamPath; set
        {
            if (ValidatePath(value))
            {
                gameParamPath = value;
                return;
            }

            throw new Exception("Bad File");
        }
    }

    public string BackupGameParamPath => GameParamPath + ".bak";
    public string PrevGameParamPath => GameParamPath + ".prev";
    public string TempGameParamPath => GameParamPath + ".temp";

    public bool SuccessfulLoad => File.Exists(GameParamPath);

    public bool ShowFieldDataType { get; set; }
    public bool ShowInternalCellNames { get; set; }
    public bool ShowDisplayCellNames => !ShowInternalCellNames;
    public bool ShowFilters { get; set; } = true;
    public bool ShowFieldType { get; set; } = false;
}
