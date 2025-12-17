using UnityEngine;

namespace MoistureUpsetRemix.Common.Logging;

internal class UnityLogger : ILogger
{
    private const string Name ="MoistureUpsetRemix";
    
    public void Log(LogLevel level, object data)
    {
        var msg = $"[{Name}] [{level.ToString()}] {data}";
        switch (level)
        {
            case LogLevel.Fatal:
            case LogLevel.Error:
                Debug.LogError(msg);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(msg);
                break;
            default:
                Debug.Log(msg);
                break;
        }
    }
}