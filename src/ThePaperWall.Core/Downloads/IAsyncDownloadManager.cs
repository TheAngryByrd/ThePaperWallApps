using System;
using System.Threading.Tasks;
using Splat;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core.Downloads
{
    public interface IAsyncDownloadManager
    {
        Task<IBitmap> DownloadImage(ImageMetaData image, IProgress<ProgressEvent> progress);    
    }
}