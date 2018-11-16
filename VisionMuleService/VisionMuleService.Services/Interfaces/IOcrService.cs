using System.Threading.Tasks;
using VisionMuleService.Services.Models;

namespace VisionMuleService.Services.Interfaces
{
    public interface IOcrService
    {
        string Service { get; }

        Task<PassportModel> ScanPassportAsync(byte[] frontImage, byte[] backImage, string fileName);
    }
}
