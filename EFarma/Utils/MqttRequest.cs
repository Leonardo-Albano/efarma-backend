using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EFarma.Utils
{
    public class MqttRequest
    {
        public static async Task<List<string>> GetReadTagCodes(HttpClient httpClient, string uniqueId)
        {
            string url = "http://157.230.224.194:5002/get_tags";
            //string url = "http://157.230.224.194:5000/TagCodes";
            try
            {
                var requestUrl = $"{url}?code={uniqueId}";

                var response = await httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var tagCodeResponse = JsonConvert.DeserializeObject<JObject>(responseBody);

                var tags = tagCodeResponse["tags"]?.ToObject<List<string>>() ?? [];

                return tags;
            }
            catch (Exception ex)
            {
                return [];
            }
        }
    }
}
