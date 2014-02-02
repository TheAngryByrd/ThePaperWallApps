using Splat;
using System;
using System.Threading.Tasks;
using Akavache;
using System.Reactive.Linq;
using Punchclock;

namespace ThePaperWall.Core.Downloads
{
    public class AsyncDownloadManager : IAsyncDownloadManager
    {
        private static OperationQueue _opQueue = new OperationQueue(4);

        public async Task<IBitmap> DownloadImage(string imageUrl, 
            IProgress<ProgressEvent> progress = null,
            int priority = 1)
        {
            try
            {
                return await _opQueue.EnqueueObservableOperation(priority, () =>
                {
                    try
                    {
                        return BlobCache.LocalMachine.LoadImageFromUrl(imageUrl,absoluteExpiration: DateTimeOffset.Now.AddDays(1));
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                });
               
            }
            catch(Exception _)
            {
                
            }
            return Splat.BitmapLoader.Current.Create(1,1);
        }

        public async Task<byte[]> Download(string url,
            int priority = 1)
        {
             return await _opQueue.EnqueueObservableOperation(priority, () =>
                {
                    try
                    {
                        return BlobCache.LocalMachine.DownloadUrl(url,absoluteExpiration: DateTimeOffset.Now.AddDays(1));
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                });
        }
    }
}


