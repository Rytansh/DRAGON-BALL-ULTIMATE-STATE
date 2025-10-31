using UnityEngine;

public static class Logging
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void System(string message)
    {
        Debug.Log($"<color=#8A2BE2>[System]</color> {message}");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Simulation(string message)
    {
        Debug.Log($"<color=#1E90FF>[Simulation]</color> {message}");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Presentation(string message)
    {
        Debug.Log($"<color=#32CD32>[Presentation]</color> {message}");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Warning(string message)
    {
        Debug.LogWarning($"<color=yellow>[Warning]</color> {message}");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Error(string message)
    {
        Debug.LogError($"<color=red>[Error]</color> {message}");
    }
}

