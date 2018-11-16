using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisionMuleService.Controllers.Enums;

namespace VisionMuleService.Controllers.ViewModels
{
    public class VerifyImageDataRequestViewModel
    {
        public string ScanId { get; set; }

        public DataVerificationStatus Status { get; set; }
    }
}
