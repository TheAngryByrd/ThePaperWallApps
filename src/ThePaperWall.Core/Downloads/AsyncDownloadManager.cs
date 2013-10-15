using Splat;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Akavache;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Punchclock;

namespace ThePaperWall.Core.Downloads
{
    public class AsyncDownloadManager : IAsyncDownloadManager
    {
        private static OperationQueue _opQueue = new OperationQueue(2);

        public Task<IBitmap> DownloadImage(string imageUrl, IProgress<ProgressEvent> progress = null)
        {
            try
            {
                return _opQueue.Enqueue(1, () => BlobCache.LocalMachine.LoadImageFromUrl(imageUrl).ToTask());
            }
            catch(Exception _)
            {
                
            }
            return Task.Run(() => Splat.BitmapLoader.Current.Create(1,1));
        }
    }
}


