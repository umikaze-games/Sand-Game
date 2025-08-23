using UnityEngine;

public static class StringUtilities 
{
    public static string ColorString(string s, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hexColor}>{s}</color>";
    }
}
