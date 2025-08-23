using System.Text;
using UnityEngine;

public static class TimeUtilities 
{
    public static string FormatTime(int seconds)
    {
        if (seconds < 0) return "Invalid time";

        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int secs = seconds % 60;

        StringBuilder result = new StringBuilder();

        if (hours > 0)
            result.Append($"{hours}h ");
        if (minutes > 0)
            result.Append($"{minutes}min ");
        if ((secs > 0 || seconds == 0) && (hours <= 0 || minutes <= 0)) // Always show seconds if it's the only value
            result.Append($"{secs}s");

        return result.ToString().Trim();
    }
}
