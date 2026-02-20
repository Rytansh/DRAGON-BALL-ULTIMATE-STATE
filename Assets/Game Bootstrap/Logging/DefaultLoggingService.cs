using UnityEngine;

public sealed class DefaultLoggingService : ILoggingService
{
    public void Info(string message)
    {
        Logging.System($"[INFO] {message}");
    }

    public void Warn(string message)
    {
        Logging.System($"<color=yellow>[WARN]</color> {message}");
    }

    public void Error(string message)
    {
        Logging.System($"<color=red>[ERROR]</color> {message}");
    }
}

