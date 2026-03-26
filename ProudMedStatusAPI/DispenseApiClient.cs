using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProudMedStatusAPI
{
    /// <summary>
    /// เรียก API endpoint /api/robot/updateDispense
    /// </summary>
    public class DispenseApiClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly LogManager _log;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public DispenseApiClient(string domainApi, LogManager log)
        {
            _log = log;

            // รับรองทั้ง http:// และ https://
            string baseUrl = domainApi.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? domainApi
                : $"http://{domainApi}";

            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
                Timeout = TimeSpan.FromSeconds(30),
            };
        }

        /// <summary>
        /// POST /api/robot/updateDispense
        /// Body: { "prescriptionItemID": "000001,000002,..." }
        /// Return: ApiResponse หรือ null ถ้า error
        /// </summary>
        public async Task<ApiResponse?> UpdateDispenseAsync(string prescriptionItemIDs)
        {
            try
            {
                var body = new { prescriptionItemID = prescriptionItemIDs };

                _log.Info($"POST /api/robot/updateDispense → IDs={prescriptionItemIDs}");
              
                using var response = await _http.PostAsJsonAsync("api/robot/updateDispense", body);
                string raw = await response.Content.ReadAsStringAsync();

                int statusCode = (int)response.StatusCode;
                _log.Info($"Response {statusCode}: {Truncate(raw, 300)}");

                if (statusCode == 201)
                {
                    return JsonSerializer.Deserialize<ApiResponse>(raw, _jsonOptions);
                }

                // 400 / 500
                _log.Error($"API error {statusCode}: {Truncate(raw, 300)}");
                return null;
            }
            catch (TaskCanceledException)
            {
                _log.Error("API call timeout (>30s)");
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"API call exception: {ex.Message}");
                return null;
            }
        }

        private static string Truncate(string s, int max) =>
            s.Length <= max ? s : s[..max] + "…";

        public void Dispose() => _http.Dispose();
    }
}