using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisionMuleService.Services.Models;

namespace VisionMuleService.Controllers.ViewModels
{
    public class ScanResultViewModel
    {
        public string ScanId { get; set; }

        public PassportModel Data { get; set; }
    }
}
