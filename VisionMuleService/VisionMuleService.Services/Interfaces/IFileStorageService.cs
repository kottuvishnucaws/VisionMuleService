using System.IO;
using System.Threading.Tasks;

namespace VisionMuleService.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> StoreImageAsync(byte[] frontImage, byte[] backImage);

        Task<bool> MoveToSubFolderAsync(string id, string subFolderName);
    }
}
