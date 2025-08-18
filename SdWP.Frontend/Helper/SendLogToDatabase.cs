using SdWP.Frontend.Enum;
using Serilog;

namespace SdWP.Frontend.Functions
{
    public class SendLogToDatabase
    {
        private readonly HttpClient _httpClient;

        public SendLogToDatabase(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task SendLogAsync(
            string errorMessage,
            string source,
            string stackTrace,
            TypeOfLog typeOfLog)
        {
            switch (typeOfLog)
            {
                case TypeOfLog.Info: Log.Information(errorMessage); break;
                case TypeOfLog.Warning: Log.Warning(errorMessage); break;
                case TypeOfLog.Error: Log.Error(errorMessage); break;
            }

            var logObj = new
            {
                Message = errorMessage,
                StackTrace = stackTrace,
                Source = source,
                TypeOfLog = typeOfLog.ToString()
            };

            await _httpClient.PostAsJsonAsync("api/ErrorLog/log", logObj);
        }
    }
}
