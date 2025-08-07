using SdWP.Frontend.Enum;
using Serilog;

namespace SdWP.Frontend.Functions
{
    public class SendLogToDatabase
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task SendLogAsync(
            string errorMessage,
            string source,
            string stackTrace,
            TypeOfLog typeOfLog)
        {
            if (typeOfLog == TypeOfLog.Info)
            {
                Log.Information(errorMessage);
            }
            else if (typeOfLog == TypeOfLog.Warning)
            {
                Log.Warning(errorMessage);
            }
            else if (typeOfLog == TypeOfLog.Error)
            {
                Log.Error(errorMessage);
            }

            var logContent = new StringContent(string.Empty);
            await _httpClient.PostAsync(
                $"api/log/Message={errorMessage}StackTrace={stackTrace}Source={source}TypeOfLog={typeOfLog}",
                logContent
            );
        }
    }
}
