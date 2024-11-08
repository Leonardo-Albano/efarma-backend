using System.Net.Http;

namespace EFarma.Utils
{
    public class MqttRequest
    {
        public static async Task<List<string>> GetReadTagCodes(HttpClient httpClient, string uniqueId)
        {
            string url = "http://157.230.224.194:5002";
            try
            {
                var requestUrl = $"{url}/get_tags?code={uniqueId}";

                var response = await httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var tagCodeResponse = JsonConvert.DeserializeObject<List<string>>(responseBody);

                return tagCodeResponse ?? [];
            }
            catch (Exception ex)
            {
                return [];
            }
        }
    }
}
