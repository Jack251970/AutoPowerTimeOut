using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AutoPowerTimeOut;

public class TimeOutModel
{
    public TimeOutLevel Value { get; private init; }
    public string Display { get; set; } = string.Empty;
    
    public static List<TimeOutModel> GetValues(bool pluggedIn)
    {
        var values = Enum.GetValues<TimeOutLevel>().Cast<TimeOutLevel>().Select(v => new TimeOutModel
        {
            Value = v,
            Display = GetDescriptionAttr(v, pluggedIn)
        });
        // Put the "Never" option at the end
        var never = values.FirstOrDefault(v => v.Value == TimeOutLevel.Never);
        if (never != null)
        {
            values = values.Where(v => v.Value != TimeOutLevel.Never).Append(never);
        }
        else
        {
            _ = values.Append(new TimeOutModel
            {
                Value = TimeOutLevel.Never,
                Display = GetDescriptionAttr(TimeOutLevel.Never, pluggedIn)
            });
        }
        return [.. values];
    }

    private const string RecommendedFormat = "{0} (Recommended)";
    private static string GetDescriptionAttr(TimeOutLevel value, bool pluggedIn)
    {
        var field = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])field?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;
        var description = attributes is { Length: > 0 } ? attributes[0].Description : value.ToString();
        // 5 minutes is recommended for plugged in
        if (pluggedIn && value == TimeOutLevel.FiveMinutes)
        {
            return string.Format(RecommendedFormat, description);
        }
        else if (!pluggedIn && value == TimeOutLevel.ThreeMinutes)
        {
            return string.Format(RecommendedFormat, description);
        }
        else
        {
            return description;
        }
    }
}

public enum TimeOutLevel
{
    [Description("1 minute")]
    OneMinute = 1,
    [Description("2 minutes")]
    TwoMinutes = 2,
    [Description("3 minutes")]
    ThreeMinutes = 3,
    [Description("5 minutes")]
    FiveMinutes = 5,
    [Description("10 minutes")]
    TenMinutes = 10,
    [Description("15 minutes")]
    FifteenMinutes = 15,
    [Description("20 minutes")]
    TwentyMinutes = 20,
    [Description("25 minutes")]
    TwentyFiveMinutes = 25,
    [Description("30 minutes")]
    ThirtyMinutes = 30,
    [Description("45 minutes")]
    FortyFiveMinutes = 45,
    [Description("1 hour")]
    OneHour = 60,
    [Description("2 hours")]
    TwoHours = 120,
    [Description("3 hours")]
    ThreeHours = 180,
    [Description("4 hours")]
    FourHours = 240,
    [Description("5 hours")]
    FiveHours = 300,
    [Description("Never")]
    Never = 0
}
