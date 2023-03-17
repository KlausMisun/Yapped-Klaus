using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FSParam.Param;
using static SoulsFormats.PARAM;

namespace Yapped.Klaus.WPF.Converters;

public class StringToCellTypeConverter
{
    public static object Convert(object value)
    {
        if (value is not Param.Cell cell)
        {
            throw new Exception("HUGE ISSUE");
        }

        return cell.Value;
    }

    public static object ConvertBack(object value, object parameter)
    {
        if (value is not string newValue)
        {
            throw new Exception();
        }

        if (parameter is not PARAMDEF.Field field)
            {
                throw new Exception();
        }

        return System.Convert.ChangeType(newValue, TypeForParamDefType(field.DisplayType));
    }

    public static Type TypeForParamDefType(PARAMDEF.DefType type, bool isArray = false)
    {
        return type switch
        {
            PARAMDEF.DefType.s8 => typeof(sbyte),
            PARAMDEF.DefType.u8 => typeof(byte),
            PARAMDEF.DefType.s16 => typeof(short),
            PARAMDEF.DefType.u16 => typeof(ushort),
            PARAMDEF.DefType.s32 or PARAMDEF.DefType.b32 => typeof(int),
            PARAMDEF.DefType.u32 => typeof(uint),
            PARAMDEF.DefType.f32 or PARAMDEF.DefType.angle32 => typeof(float),
            PARAMDEF.DefType.f64 => typeof(double),
            PARAMDEF.DefType.dummy8 => isArray ? typeof(byte[]) : typeof(byte),
            PARAMDEF.DefType.fixstr or PARAMDEF.DefType.fixstrW => typeof(string),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    public static bool CanConvertToParamDefType(string value, PARAMDEF.DefType type)
    {
        var success = type switch
        {
            PARAMDEF.DefType.s8 => sbyte.TryParse(value, out _),
            PARAMDEF.DefType.u8 => byte.TryParse(value, out _),
            PARAMDEF.DefType.s16 => short.TryParse(value, out _),
            PARAMDEF.DefType.u16 => ushort.TryParse(value, out _),
            PARAMDEF.DefType.s32 or PARAMDEF.DefType.b32 => int.TryParse(value, out _),
            PARAMDEF.DefType.u32 => uint.TryParse(value, out _),
            PARAMDEF.DefType.f32 or PARAMDEF.DefType.angle32 => float.TryParse(value, out _),
            PARAMDEF.DefType.f64 => double.TryParse(value, out _),
            PARAMDEF.DefType.dummy8 => byte.TryParse(value, out _),
            PARAMDEF.DefType.fixstr or PARAMDEF.DefType.fixstrW => true,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        return success; 
    }
}
