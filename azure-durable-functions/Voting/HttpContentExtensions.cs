using System.Net.Http;
using System.Threading.Tasks;

namespace Voting
{
    public static class HttpContentExtensions
    {
        public static async Task<T> Deserialize<T>(this HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            using (var textReader = new System.IO.StreamReader(stream))
            using (var reader = new Newtonsoft.Json.JsonTextReader(textReader))
            {
                return Newtonsoft.Json.JsonSerializer.CreateDefault().Deserialize<T>(reader);
            }
        }
    }
}