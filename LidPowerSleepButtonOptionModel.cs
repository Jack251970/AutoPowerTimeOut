using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AutoPowerTimeOut;

public class LidPowerSleepButtonOptionModel
{
    public LidPowerSleepButtonOption Value { get; private init; }
    public string Display { get; set; } = string.Empty;

    public static List<LidPowerSleepButtonOptionModel> GetValues()
    {
        var values = Enum.GetValues<LidPowerSleepButtonOption>().Cast<LidPowerSleepButtonOption>().Select(v => new LidPowerSleepButtonOptionModel
        {
            Value = v,
            Display = GetDescriptionAttr(v)
        });
        return [.. values];
    }

    private static string GetDescriptionAttr(LidPowerSleepButtonOption value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])field?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;
        var description = attributes is { Length: > 0 } ? attributes[0].Description : value.ToString();
        return description;
    }
}

public enum LidPowerSleepButtonOption
{
    [Description("Do Nothing")]
    DoNothing = 0,
    [Description("Sleep")]
    Sleep = 1,
    [Description("Hibernate")]
    Hibernate = 2,
    [Description("Shut-down")]
    ShutDown = 3
}
