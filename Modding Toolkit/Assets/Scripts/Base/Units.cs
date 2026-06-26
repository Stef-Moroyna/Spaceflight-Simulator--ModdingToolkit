using UnityEngine;
using System;
using System.Globalization;
using System.Linq;
using SFS.Translations;
using SFS.Input;

public static class Units
{
    // Makes sure numbers are displayed as 10000 and not 10'000
    static Units()
    {
        DecimalsFormat = new CultureInfo("en-US", false).NumberFormat;
        DecimalsFormat.NumberGroupSeparator = "";
    }
    static readonly NumberFormatInfo DecimalsFormat;

    public static string ToFundsString(this double a)
    {
        if (a == 0)
            return "$0";
        
        return "$" + a.ToString(3, true).Replace(".", ",") + ",000";
    }
    public static string ToDistanceString(this double a, bool decimals = true)
    {
        if (double.IsInfinity(a))
            return Loc.main.Escape;

        double abs = Math.Abs(a);

        if (abs < 1000)
            return a.ToString(decimals ? 1 : 0, true) + Loc.main.Meter_Unit;

        if (abs < 10000)
            return a.ToString(0, true) + Loc.main.Meter_Unit;

        return (a / 1000).ToString(decimals ? 1 : 0, true) + Loc.main.Km_Unit;
    }
    public static string ToKmString(this int a)
    {
        return (a / 1000) + Loc.main.Km_Unit;
    }
    public static string ToVelocityString(this double a, bool decimals = true, bool doubleDecimal = false)
    {
        return a.ToString(decimals ? (doubleDecimal && a < 10? 2 : 1) : 0, true) + Loc.main.Meter_Per_Second_Unit;
    }
    public static string ToMassString(this float a, bool forceDecimal, int decimalCount = 2)
    {
        return Loc.main.Mass.Inject(a.ToString(decimalCount, forceDecimal), "value");
    }
    public static string ToThrustString(this float a)
    {
        return Loc.main.Thrust.Inject(a.ToString(2, false), "value");
    }
    public static string ToBurnTimeString(this double a, bool forceDecimals)
    {
        return Loc.main.Burn_Time.Inject(a.ToString(1, forceDecimals), "value");
    }
    public static string ToTwrString(this float a)
    {
        return Loc.main.Thrust_To_Weight_Ratio.Inject(a.ToString(2, true), "value");
    }
    public static string ToEfficiencyString(this float a)
    {
        return Loc.main.Efficiency.Inject(a.ToString(1, false), "value");
    }
    public static string ToSeparationForceString(this float a, bool forceDecimals)
    {
        return Loc.main.Separation_Force.Inject(a.ToString(1, forceDecimals), "value");
    }
    public static string ToMagnetForceString(this float a, bool forceDecimals)
    {
        return Loc.main.Magnet_Force.Inject(a.ToString(1, forceDecimals), "value");
    }
    public static string ToTemperatureString(this float a)
    {
        return (int)a + "°C";
    }
    public static string ToTimestampString(this double seconds, bool showSeconds, bool showInBetweenUnitsThatAreZero)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        
        string timestamp = (int)timeSpan.TotalDays > 0? (Loc.main.Day_Short.Inject(((int)timeSpan.TotalDays).ToString(), "value") + " ") : "";
        timestamp += (showInBetweenUnitsThatAreZero? (int)timeSpan.TotalHours : timeSpan.Hours) > 0 ? (Loc.main.Hour_Short.Inject(timeSpan.Hours.ToString(), "value") + " ") : "";
        timestamp += !showSeconds || (showInBetweenUnitsThatAreZero? (int)timeSpan.TotalMinutes : timeSpan.Minutes) > 0 ? (Loc.main.Minute_Short.Inject(timeSpan.Minutes.ToString(), "value") + " ") : "";
        timestamp += showSeconds && ((showInBetweenUnitsThatAreZero? (int)timeSpan.TotalSeconds : timeSpan.Seconds) > 0)? (Loc.main.Second_Short.Inject(timeSpan.Seconds.ToString(), "value") + " ") : "";

        if (timestamp.Length > 0 && timestamp.Last() == ' ')
            timestamp = timestamp.Remove(timestamp.Length - 1, 1);
        
        return timestamp;
    }
    public static string ToPercentString(this float a, bool forceDecimals = true)
    {
        return ToPercentString((double)a, forceDecimals);
    }
    public static string ToPercentString(this double a, bool forceDecimals = true)
    {
        a *= 100;

        if (a >= 9.5)
            return a.ToString(0, false) + "%"; // 10-100% --> 0 decimals

        if (a >= 0.1)
            return a.ToString(1, forceDecimals) + "%"; // 0.1-10% --> 1 decimal

        return a > 0 ? "0.1%" : "0%";
    }
    public static string ToLastPlayedString(this TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 30)
            return Loc.main.Just_Played;
        
        return Loc.main.Last_Played.Inject(timeSpan.TotalSeconds.ToTimestampString(timeSpan.TotalMinutes < 5, true), "value");
    }
    public static string ToTimePlayedString(this TimeSpan timeSpan)
    {
        string timestamp = Loc.main.Time_Played;
        timestamp = timestamp.Replace("%value%", Loc.main.Hour_Short.Inject(Math.Round(timeSpan.TotalHours, 1).ToString(CultureInfo.InvariantCulture), "value"));
        return timestamp;
    }

    public static string ToKeysString(this KeyCode[] keyCodes) => $"[{string.Join(",", keyCodes.Select(kc => kc.ToString()))}]";
    public static string ToKeysString(this KeybindingsPC.Key[] keys) => keys.Select(k => k.key).ToArray().ToKeysString();

    public static string ToString(this float a, int decimals, bool forceDecimals)
    {
        return ((double)a).ToString(decimals, forceDecimals);
    }
    public static string ToString(this double a, int decimals, bool forceDecimals)
    {
        a = a.Round(decimals);

        // Displays 5.0 instead of 5
        return forceDecimals ? a.ToString("N" + decimals, DecimalsFormat) : a.ToString(DecimalsFormat);
    }

    public static float Round(this float value, int decimals)
    {
        float ratio = Mathf.Pow(10, decimals);
        return Mathf.Round(value * ratio) / ratio;
    }
    public static double Round(this double value, int decimals)
    {
        float ratio = Mathf.Pow(10, decimals);
        return Math.Round(value * ratio) / ratio;
    }
}