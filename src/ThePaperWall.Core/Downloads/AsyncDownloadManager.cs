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

        public async Task<IBitmap> DownloadImage(string imageUrl, IProgress<ProgressEvent> progress = null)
        {
            try
            {
                return await _opQueue.EnqueueObservableOperation(1, () =>
                {
                    try
                    {
                        return BlobCache.LocalMachine.LoadImageFromUrl(imageUrl);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                });
                //return await BlobCache.LocalMachine.LoadImageFromUrl(imageUrl);
                return await _opQueue.Enqueue(1, () =>
                {
                    
                    try
                    {
                        return BlobCache.LocalMachine.LoadImageFromUrl(imageUrl).ToTask();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    });
            }
            catch(Exception _)
            {
                
            }
            return Splat.BitmapLoader.Current.Create(1,1);
        }
    }
}


