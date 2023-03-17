using FSParam;
using SoulsFormats;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Yapped.Klaus.Core.Data;

namespace Yapped.Klaus.Core.DataHelper;

public class DataManagers
{
    public DataManagers()
    {

    }

    private static Dictionary<string, PARAMDEF> LoadParamDefinitions()
    {
        Dictionary<string, PARAMDEF> paramdefs = new();

        string[] files = Directory.GetFiles(AppConfiguration.ParamDefsFolderPath, "*.xml");

        foreach (var file in files)
        {
            var paramDef = PARAMDEF.XmlDeserialize(file);
            paramdefs.Remove(paramDef.ParamType);
            paramdefs.Add(paramDef.ParamType, paramDef);
        }

        return paramdefs;
    }

    private static Dictionary<int, string> LoadParamRowNames(string trimmedFileName)
    {
        var nameFile = Path.Combine(AppConfiguration.ParamNamesFolderPath, $"{trimmedFileName}.txt");

        Dictionary<int, string> nameDictionary = new();

        if (File.Exists(nameFile))
        {
            var nameLines = File.ReadAllLines(nameFile);

            foreach (var nameLine in nameLines)
            {
                // having the split trim any excess spaces in between fields as well as removing all empty entires
                var fields = nameLine.Split(' ', 2, StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries);

                if (fields.Length < 2)
                {
                    continue;
                }

                if (!int.TryParse(fields[0], out int id))
                {
                    continue;
                }

                var name = fields[1];

                nameDictionary.TryAdd(id, name);
            }
        }

        return nameDictionary;
    }

    public static Task<IEnumerable<PARAMAdapter>> LoadBNDParamsAsync(string gameParamPath) => Task.Run(() =>
        {
            var paramDefinitions = LoadParamDefinitions();
            var paramBank = new List<PARAMAdapter>(); IBinder paramBndData = BND4.Read(gameParamPath);

            var paramFiles = paramBndData.Files.Where(x => x.Name.EndsWith(".param", StringComparison.OrdinalIgnoreCase));

            foreach (var file in paramFiles)
            {
                string trimmedFileName = Path.GetFileNameWithoutExtension(file.Name);

                if (paramBank.Any(x => x.ParamName == trimmedFileName))
                {
                    continue;
                }

                if (trimmedFileName.EndsWith("LoadBalancerParam"))
                {
                    continue;
                }

                Param param = Param.Read(file.Bytes);

                if (!paramDefinitions.ContainsKey(param.ParamType))
                {
                    continue;
                }

                var paramDefinition = paramDefinitions[param.ParamType];

                param.ApplyParamdef(paramDefinition);

                Dictionary<int, string> nameDictionary = LoadParamRowNames(trimmedFileName);

                PARAMAdapter adaptedParam = new(nameDictionary, param,
                    trimmedFileName);

                paramBank.Add(adaptedParam);

            }

            return (IEnumerable<PARAMAdapter>)paramBank;
        });

    public static Task SaveBNDAync(IEnumerable<PARAMAdapter> paramList, string gameParamPath) => Task.Run(() =>
        {
            // we use this incase we decide to load a different file in a different fashion
            // just in case
            var backUpGameParamPath = gameParamPath + ".bak";
            var prevGameParamPath = gameParamPath + ".prev";
            var tempGameParamPath = gameParamPath + ".temp";

            if (!File.Exists(backUpGameParamPath))
            {
                File.Copy(gameParamPath, backUpGameParamPath);
            }


            BND4 paramBnd = BND4.Read(gameParamPath);

            foreach (var file in paramBnd.Files)
            {
                PARAMAdapter? paramFile = paramList.FirstOrDefault(x => x.ParamName == Path.GetFileNameWithoutExtension(file.Name));

                if (paramFile is not null)
                {
                    var bytes = paramFile.Param.Write();
                    file.Bytes = bytes;
                }

            }

            if (File.Exists(tempGameParamPath))
            {
                File.Delete(tempGameParamPath);
            }

            paramBnd.Write(tempGameParamPath);

            if (File.Exists(gameParamPath))
            {
                File.Copy(gameParamPath, prevGameParamPath, true);
                File.Delete(gameParamPath);
            }

            File.Move(tempGameParamPath, gameParamPath);
        });

    public static void CreateBNDBackup(string gameParamPath, string backUpGameParamPath)
    {
        if (!File.Exists(backUpGameParamPath))
        {
            File.Copy(gameParamPath, backUpGameParamPath);
        }
    }
}

