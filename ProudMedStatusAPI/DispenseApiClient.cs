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
        private readonly string _fullUrl;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public DispenseApiClient(string domainApi, LogManager log)
        {
            _log = log;

            string baseUrl = domainApi.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? domainApi
                : $"http://{domainApi}";

            // ต่อ full URL ตรงๆ แทนการใช้ BaseAddress + relative path
            _fullUrl = baseUrl.TrimEnd('/') + "/api/robot/updateDispense";

           // _log.Info($"API URL = {_fullUrl}");

            _http = new HttpClient
            {
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
                var body = new { PrescriptionItemID = prescriptionItemIDs };

                // Log body ที่จะส่ง
                string bodyJson = JsonSerializer.Serialize(body);
                _log.Info($"POST {_fullUrl}");
                _log.Info($"Request body: {bodyJson}");

                using var request = new HttpRequestMessage(HttpMethod.Post, _fullUrl);
                request.Content = new StringContent(bodyJson, System.Text.Encoding.UTF8, "application/json");

                using var response = await _http.SendAsync(request);

                string raw = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                // Log ทุกอย่างออกมา
                _log.Info($"StatusCode: {statusCode}");
                _log.Info($"Raw response: [{raw}]");  // ใส่ [] เพื่อดูว่า whitespace หรือเปล่า
                _log.Info($"Content-Type: {response.Content.Headers.ContentType}");
                _log.Info($"Content-Length: {response.Content.Headers.ContentLength}");

                if (statusCode == 201)
                {
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        _log.Error("201 แต่ body ว่างเปล่า!");
                        return null;
                    }

                    try
                    {
                        var result = JsonSerializer.Deserialize<ApiResponse>(raw, _jsonOptions);
                        _log.Info($"Deserialize OK: status={result?.Status}, message count={result?.Message?.Count}");
                        return result;
                    }
                    catch (JsonException jex)
                    {
                        _log.Error($"Deserialize failed: {jex.Message}");
                        return null;
                    }
                }

                _log.Error($"API error {statusCode}: {raw}");
                return null;
            }
            catch (TaskCanceledException)
            {
                _log.Error("Timeout >30s");
                return null;
            }
            catch (HttpRequestException hrex)
            {
                _log.Error($"HttpRequest error: {hrex.Message}");  // จับ network error แยก
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Exception: {ex}");
                return null;
            }
        }
        public async Task<bool> PingAsync()
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Head, _fullUrl);
                using var res = await _http.SendAsync(req);
                // ถ้า server ตอบกลับมา (ไม่ว่า status code อะไร) = ถือว่า reachable
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string Truncate(string s, int max) =>
            s.Length <= max ? s : s[..max] + "…";

        public void Dispose() => _http.Dispose();
    }
}