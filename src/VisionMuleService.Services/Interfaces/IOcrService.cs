using System.Threading.Tasks;
using VisionMuleService.Services.Models;

namespace VisionMuleService.Services.Interfaces
{
    public interface IOcrService
    {
        Task<PassportModel> ScanPassportAsync(byte[] image);
    }
}
