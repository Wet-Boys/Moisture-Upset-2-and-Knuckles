namespace MoistureUpsetRemix.Common.Logging;

public static class Log
{
    private static ILogger _logger = new UnityLogger();

    public static void SetLogger(ILogger logger)
    {
        _logger = logger;
    }
    
    public static void Fatal(object data) => _logger.Log(LogLevel.Fatal, data);
    
    public static void Error(object data) => _logger.Log(LogLevel.Error, data);
    
    public static void Warning(object data) => _logger.Log(LogLevel.Warning, data);
    
    public static void Message(object data) => _logger.Log(LogLevel.Message, data);
    
    public static void Info(object data) => _logger.Log(LogLevel.Info, data);
    
    public static void Debug(object data) => _logger.Log(LogLevel.Debug, data);
    
}