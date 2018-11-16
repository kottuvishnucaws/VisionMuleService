using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisionMuleService.Services.Interfaces;
using VisionMuleService.Services.Models;

namespace VisionMuleService.Services.Services
{
    public class GoogleVisionService : IOcrService
    {
        public string Service { get; } = "GoogleVision";

        public async Task<PassportModel> ScanPassportAsync(byte[] frontImage, byte[] backImage = null, string fileName = null)
        {
            var passport = new PassportModel();

            try
            {
                if (frontImage == null)
                    return passport;

                dynamic credential;
                using (var stream = new FileStream("visioncredentails.json", FileMode.Open, FileAccess.Read))
                {
                    string[] scopes = { VisionService.Scope.CloudPlatform };
                    credential = GoogleCredential.FromStream(stream);
                    credential = credential.CreateScoped(scopes);
                }

                var service = new VisionService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "VisionMule",
                    GZipEnabled = true,
                });

                BatchAnnotateImagesRequest batchRequest = new BatchAnnotateImagesRequest();
                batchRequest.Requests = new List<AnnotateImageRequest>();

                batchRequest.Requests.Add(new AnnotateImageRequest()
                {
                    Features = new List<Feature>() { new Feature() { Type = "DOCUMENT_TEXT_DETECTION", MaxResults = 1 }, },
                    ImageContext = new ImageContext() { LanguageHints = new List<string>() { "en" } },
                    Image = new Image() { Content = Convert.ToBase64String(frontImage) }
                });

                if(backImage != null)
                {
                    batchRequest.Requests.Add(new AnnotateImageRequest()
                    {
                        Features = new List<Feature>() { new Feature() { Type = "DOCUMENT_TEXT_DETECTION", MaxResults = 1 }, },
                        ImageContext = new ImageContext() { LanguageHints = new List<string>() { "en" } },
                        Image = new Image() { Content = Convert.ToBase64String(backImage) }
                    });
                }


                var annotate = service.Images.Annotate(batchRequest);
                BatchAnnotateImagesResponse batchAnnotateImagesResponse =await annotate.ExecuteAsync();
                try
                {
                    string[] passportFrontData = batchAnnotateImagesResponse.Responses[0].FullTextAnnotation.Text.Split('\n');
                    passportFrontData = passportFrontData.Where(x => x.ToString().Length > 1).ToArray();
                    string[] passportSortedDetails = passportFrontData.Where(x => x.ToString().Contains("<<")).ToArray();

                    if (passportSortedDetails.Length > 1)
                    {
                        string[] p1 = passportSortedDetails[0].Replace('<', ' ').Trim().Split(' ');
                        string[] p2 = passportSortedDetails[1].Replace('<', ' ').Trim().Split(' ');
                        passport.LastName = p1[1].Trim().Contains("IND") && p1.Length > 3 ? p1[1].Trim().Remove(0, 3) : p1.Any(p => p.Contains("IND")) && p1[Array.FindIndex(p1, i => i.Contains("IND"))].Length > 3 ? p1[Array.FindIndex(p1, i => i.Contains("IND"))].Remove(0, 3) : null;
                        if (p1.Length >= 2)
                        {
                            for (int i = 2; i < p1.Length; i++)
                                passport.FirstName += " " + p1[i];
                        }
                        passport.FirstName = passport.FirstName.Trim();
                        passport.Number = p2[0].Trim().Length > 3 ? p2[0].Trim() : null;

                    }
                    DateTime dob = DateTime.MinValue;

                    int placeOfBirthIndex = Array.FindIndex(passportFrontData, i => i.Contains("Place of Birth")) + 1;
                    int placeOfIssueIndex = Array.FindIndex(passportFrontData, i => i.Contains("Place of Issue")) + 1;
                    int dobIndex = Array.FindIndex(passportFrontData, i => i.Contains("Date of Birth"));

                    for (int p = 1; p < 5; p++)
                    {
                        bool isValid = DateTime.TryParseExact(passportFrontData[dobIndex + p], "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
                        if (isValid)
                            break;
                    }

                    passport.DateOfBirth = dob > DateTime.MinValue ? dob.ToString("dd-MM-yyyy") : string.Empty;
                    passport.PlaceOfBirth = passportFrontData[placeOfBirthIndex];
                    passport.PlaceOfIssue = passportFrontData[placeOfIssueIndex];
                }
                catch (Exception) { }

                string[] passportBackData = null;
                if (backImage != null)
                {
                    passportBackData = batchAnnotateImagesResponse.Responses[1].FullTextAnnotation.Text.Split('\n');
                    passportBackData = passportBackData.Where(x => x.ToString().Length > 1).ToArray();

                }

                if (passportBackData != null)
                {
                    int fatherName = Array.FindIndex(passportBackData, i => i.Contains("Name of Father")) + 1;
                    int motherName = Array.FindIndex(passportBackData, i => i.Contains("Name of Mother")) + 1;
                    int spouseName = Array.FindIndex(passportBackData, i => i.Contains("Name of Spouse")) + 1;
                    int Address = Array.FindIndex(passportBackData, i => i.Contains("Address")) + 1;

                    passport.FatherName = passportBackData[fatherName];
                    passport.MotherName = motherName > 1 ? passportBackData[motherName] : passportBackData[motherName + 1];
                    passport.SpouseName = spouseName > 3 ? passportBackData[spouseName] : passportBackData[spouseName + 2];
                    passport.Address = Address > 5 ? (passportBackData[Address] + passportBackData[Address + 1] + passportBackData[Address + 2]).Replace('\n', ',') 
                                                    : (passportBackData[Address + 3] + passportBackData[Address + 4] + passportBackData[Address + 5]).Replace('\n', ',');
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();

                Console.WriteLine("Error: Scanning Image."+ex.Message+line);
            }
            return passport;
        }
    }
}
