namespace MoistureUpsetRemix.Common.Logging;

public interface ILogger
{
    public void Log(LogLevel level, object data);
}