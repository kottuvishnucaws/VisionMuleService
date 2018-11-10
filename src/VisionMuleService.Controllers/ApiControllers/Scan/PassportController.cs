using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisionMuleService.Controllers.ViewModels;
using VisionMuleService.Services.Interfaces;
using VisionMuleService.Services.Services;

namespace VisionMuleService.Controllers.ApiControllers.Scan
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassportController : ControllerBase
    {
        // GET: api/Passport
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Passport/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Passport
        [HttpPost]
        public async Task<ScanResultViewModel> Post()
        {
            var imageStream = Request.HttpContext.Request.Form.Files["file"].OpenReadStream();
            Int32 length = imageStream.Length > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(imageStream.Length);
            Byte[] imageBytes = new Byte[length];
            imageStream.Read(imageBytes, 0, length);

            IFileStorageService fsService = new AzureBlobStorageService();
            ScanResultViewModel scanResult = new ScanResultViewModel();
            scanResult.ScanId =await fsService.Store(imageStream);

            IOcrService ocrService = new OcrSpaceService();
            scanResult.Data = await ocrService.ScanPassportAsync(imageBytes);

            return scanResult;
        }
    }
}
