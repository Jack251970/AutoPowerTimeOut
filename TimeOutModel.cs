using System.ComponentModel;

namespace AutoPowerTimeOut;

public class TimeOutModel
{
    public TimeOutLevel Value { get; private init; }
    public string Display { get; set; } = string.Empty;
    
    private const string RecommendedFormat = "{0} (Recommended)";
    public static List<TimeOutModel> GetValues(bool pluggedIn)
    {
        var values = Enum.GetValues<TimeOutLevel>().Cast<TimeOutLevel>().Select(v => new TimeOutModel
        {
            Value = v,
            Display = GetDescriptionAttr(v)
        }).ToList();
        if (pluggedIn)
        {
            // 5 minutes is recommended for plugged in
            foreach (var item in values)
            {
                if (item.Value == TimeOutLevel.FiveMinutes)
                {
                    item.Display = string.Format(RecommendedFormat, item.Display);
                }
            }
        }
        else
        {
            // 3 minutes is recommended for on battery
            foreach (var item in values)
            {
                if (item.Value == TimeOutLevel.ThreeMinutes)
                {
                    item.Display = string.Format(RecommendedFormat, item.Display);
                }
            }
        }
        return values;
    }

    private static string GetDescriptionAttr(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])field?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;
        return attributes is { Length: > 0 } ? attributes[0].Description : value.ToString();
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
    Never = 9999
}
