using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisionMuleService.Controllers.Models;
using VisionMuleService.Controllers.ViewModels;
using VisionMuleService.Services.Interfaces;
using VisionMuleService.Services.Services;

namespace VisionMuleService.Controllers.ApiControllers.Scan
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyController : ControllerBase
    {
        // POST: api/Verify
        [HttpPost]
        public void Post([FromBody] VerifyImageDataRequestViewModel request)
        {
            IFileStorageService fsService = new AzureBlobStorageService();

            fsService.MoveToSubFolder(request.ScanId, request.Status.ToString());
        }
    }
}
