using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VisionMuleService.Services.Interfaces;
using VisionMuleService.Services.Models;

namespace VisionMuleService.Services.Services
{
    public class OcrSpaceService : IOcrService
    {
        //Service Name Should be Same as Azure SubFolder Name.
        public string Service { get; } = "OcrSpace";

        public async Task<PassportModel> ScanPassportAsync(byte[] frontImage, byte[] backImage = null, string fileName = null)
        {
            var passport = new PassportModel();
            try
            {
                var formContent = new MultipartFormDataContent
                {
                    {new StringContent("9c5c3b7c9c88957"),"apikey"},
                    {new StringContent("eng"),"language" },
                    {new StringContent("false"),"isOverlayRequired" },
                    {new StreamContent(new MemoryStream(frontImage)),"file",fileName},
                    {new StringContent(fileName.Split('.')[1]),"filetype"}
                };
                var myHttpClient = new HttpClient();
                var response = await myHttpClient.PostAsync("https://api.ocr.space/parse/image", formContent);
                string stringContent = await response.Content.ReadAsStringAsync();

                JObject imageData = JObject.Parse(stringContent);
                string[] imageText = imageData["ParsedResults"][0]["ParsedText"].ToString().Split("\r\n");
                passport.Number = imageText[2];
                passport.LastName = imageText[3];
                passport.FirstName = imageText[4];
            }
            catch(Exception ex)
            {
            }
            return passport;
        }
    }
}
