using System;
using System.Threading.Tasks;

namespace AdvanceClient.Services
{
    /// <summary>
    /// Service interface for logging operations
    /// </summary>
    public interface ILoggingService
    {
        Task LogInfoAsync(string message, string className, string methodName);
        Task LogErrorAsync(string message, Exception? exception, string className, string methodName);
        Task LogWarningAsync(string message, string className, string methodName);
    }

    /// <summary>
    /// Implementation of logging service
    /// </summary>
    public class LoggingService : ILoggingService
    {
        public Task LogInfoAsync(string message, string className, string methodName)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] [{className}.{methodName}] {message}");
            return Task.CompletedTask;
        }

        public Task LogErrorAsync(string message, Exception? exception, string className, string methodName)
        {
            var errorMessage = $"[ERROR] [{className}.{methodName}] {message}";
            if (exception != null)
            {
                errorMessage += $"\nException: {exception.Message}\nStackTrace: {exception.StackTrace}";
            }
            System.Diagnostics.Debug.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        public Task LogWarningAsync(string message, string className, string methodName)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] [{className}.{methodName}] {message}");
            return Task.CompletedTask;
        }
    }
}
