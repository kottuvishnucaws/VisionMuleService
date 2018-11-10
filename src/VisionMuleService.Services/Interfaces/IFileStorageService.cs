using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisionMuleService.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> Store(Stream image);

        void MoveToSubFolder(string id, string subFolderName);
    }
}
