using System;
using System.Threading.Tasks;
using Akavache;
using Akavache;
using Splat;

namespace ThePaperWall.Core.Downloads
{
    public interface IAsyncDownloadManager
    {
        Task<byte[]> Download(string url,
            int priority = 1);

        Task<IBitmap> DownloadImage(string imageUrl,
            IProgress<ProgressEvent> progress = null,
            int priority = 1);    
    }
}