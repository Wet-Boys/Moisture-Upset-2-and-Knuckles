using BepInEx.Logging;
using ILogger = MoistureUpsetRemix.Common.Logging.ILogger;
using LogLevel = MoistureUpsetRemix.Common.Logging.LogLevel;

namespace MoistureUpsetRemix.Utils;

internal class Logger(ManualLogSource logSource) : ILogger
{
    public void Log(LogLevel level, object data)
    {
        switch (level)
        {
            case LogLevel.Fatal:
                logSource.LogFatal(data);
                break;
            case LogLevel.Error:
                logSource.LogError(data);
                break;
            case LogLevel.Warning:
                logSource.LogWarning(data);
                break;
            case LogLevel.Message:
                logSource.LogMessage(data);
                break;
            case LogLevel.Info:
                logSource.LogInfo(data);
                break;
            case LogLevel.Debug:
                logSource.LogDebug(data);
                break;
            default:
                logSource.Log(BepInEx.Logging.LogLevel.None, data);
                break;
        }
    }
}