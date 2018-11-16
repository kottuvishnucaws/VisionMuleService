using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisionMuleService.Controllers.ViewModels;
using VisionMuleService.Services.Interfaces;
using VisionMuleService.Services.Services;

namespace VisionMuleService.Controllers.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Scan()
        {
            return new string[] { "Error..!Use this For posting...!" };
        }

        [HttpPost]
        public async Task<ScanResultViewModel> Passport()
        {
            Byte[] frontImageBytes =null, backImageBytes = null;
            string fileName = string.Empty;
            try
            {
                if (Request.HttpContext.Request.Form.Files["frontside"].FileName != null)
                {
                    var imageFrontStream = Request.HttpContext.Request.Form.Files["frontside"].OpenReadStream();
                    fileName = Request.HttpContext.Request.Form.Files["frontside"].FileName;
                    Int32 forntLength = imageFrontStream.Length > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(imageFrontStream.Length);
                    frontImageBytes = new Byte[forntLength];
                    imageFrontStream.Read(frontImageBytes, 0, forntLength);
                }
            }
            catch (Exception) { }

            try
            {
                if (Request.HttpContext.Request.Form.Files["backside"].FileName != null)
                {
                    var imageBackStream = Request.HttpContext.Request.Form.Files["backside"].OpenReadStream();
                    Int32 backLength = imageBackStream.Length > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(imageBackStream.Length);
                    backImageBytes = new Byte[backLength];
                    imageBackStream.Read(backImageBytes, 0, backLength);
                }
            }
            catch (Exception) { }

            IOcrService ocrService = new GoogleVisionService();
            string serviceName = ocrService.Service;

            IFileStorageService fsService = new AzureBlobStorageService(serviceName);
            ScanResultViewModel scanResult = new ScanResultViewModel();

                scanResult.ScanId = await fsService.StoreImageAsync(frontImageBytes, backImageBytes);
            scanResult.Data = await ocrService.ScanPassportAsync(frontImageBytes, backImageBytes, fileName);
            return scanResult;
        }

        [HttpPost]
        public void Verify([FromBody] VerifyImageDataRequestViewModel request)
        {
            IOcrService ocrService = new GoogleVisionService();
            string serviceName = ocrService.Service;
            IFileStorageService fsService = new AzureBlobStorageService(serviceName);
            fsService.MoveToSubFolderAsync(request.ScanId, request.Status.ToString());
        }
    }
}