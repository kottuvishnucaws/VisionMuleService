using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VisionMuleService.Services.Interfaces;
using VisionMuleService.Services.Models;

namespace VisionMuleService.Services.Services
{
    public class OcrSpaceService : IOcrService
    {
        public async Task<PassportModel> ScanPassportAsync(byte[] image)
        {
            var formContent = new MultipartFormDataContent
            {
                {new StringContent("9c5c3b7c9c88957"),"apikey"},
                {new StringContent("eng"),"language" },
                {new StringContent("false"),"isOverlayRequired" },
                {new StreamContent(new MemoryStream(image)),"file","kartikpassport.jpg"},
                {new StringContent("jpg"),"filetype"}
            };
            var passport = new PassportModel();
            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync("https://api.ocr.space/parse/image", formContent);
            string stringContent = await response.Content.ReadAsStringAsync();

            JObject imageData = JObject.Parse(stringContent);
            passport.FirstName = imageData["ParsedResults"][0]["ParsedText"].ToString();

            return passport;
        }
    }
}
