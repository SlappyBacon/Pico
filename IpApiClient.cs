using Newtonsoft.Json;
using Pico.MiscTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking
{
    class IpApi : IDisposable
    {
        string requestTemplate = "http://ip-api.com/json/{IP}?fields=2114102";

        HttpClient _httpClient = new HttpClient();
        HttpClient HttpClient { get { return _httpClient; } }

        //15 requests per minute max!
        FrequencyLimiter _limiter = new FrequencyLimiter(15, 60001);
        FrequencyLimiter Limiter { get { return _limiter; } }


        public Response GetAllInfo(string ip, int timeoutMs = int.MaxValue)
        {
            var task = GetAllInfoAsync(ip, timeoutMs);
            task.Wait();
            return task.Result;
        }
        public Response GetAllInfo(string ip, CancellationToken ct)
        {
            var task = GetAllInfoAsync(ip, ct);
            task.Wait();
            return task.Result;
        }
        public async Task<Response> GetAllInfoAsync(string ip, int timeoutMs = int.MaxValue)
        {
            CancellationTokenSource cts = new CancellationTokenSource(timeoutMs);
            return await GetAllInfoAsync(ip, cts.Token);
            cts.Dispose();
        }
        public async Task<Response> GetAllInfoAsync(string ip, CancellationToken ct)
        {
            Limiter.Next(ct);
            if (ct.IsCancellationRequested) return new Response();

            var request = requestTemplate.Replace("{IP}", ip);

            var got = await HttpClient.GetAsync(request, ct);
            if (ct.IsCancellationRequested) return new Response();

            var json = await got.Content.ReadAsStringAsync(ct);
            if (ct.IsCancellationRequested) return new Response();

            var response = JsonConvert.DeserializeObject<Response>(json);

            return response;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public struct Response
        {
            [JsonProperty] string status;
            [JsonProperty] string continentCode;
            [JsonProperty] string countryCode;
            [JsonProperty] string region;
            [JsonProperty] string city;
            [JsonProperty] string zip;
            [JsonProperty] string isp;
            [JsonIgnore] public string Status { get { return status; } }
            [JsonIgnore] public string Continent { get { return continentCode; } }
            [JsonIgnore] public string Country { get { return countryCode; } }
            [JsonIgnore] public string Region { get { return region; } }
            [JsonIgnore] public string City { get { return city; } }
            [JsonIgnore] public string ZIP { get { return zip; } }
            [JsonIgnore] public string ISP { get { return isp; } }
        }
    }
}
