using Splat;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Akavache;
using System.Reactive.Linq;

namespace ThePaperWall.Core.Downloads
{
    public class AsyncDownloadManager : IAsyncDownloadManager
    {
        public static Dictionary<string, IBitmap> cache = new Dictionary<string, IBitmap>();

        public async Task<IBitmap> DownloadImage(string imageUrl, IProgress<ProgressEvent> progress = null)
        {
            return await BlobCache.LocalMachine.LoadImageFromUrl(imageUrl);
        }
    }
}


