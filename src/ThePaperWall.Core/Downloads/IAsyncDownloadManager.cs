using System;
using System.Threading.Tasks;
using Splat;

namespace ThePaperWall.Core.Downloads
{
    public interface IAsyncDownloadManager
    {
        Task<IBitmap> DownloadImage(string imageUrl, 
            IProgress<ProgressEvent> progress = null,
            int priority = 1);    
    }
}