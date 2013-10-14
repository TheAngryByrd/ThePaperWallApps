using System.Reactive.Linq;
using Akavache;
using Splat;
using System;
using System.Net.Http;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ThePaperWall.Core.Models;
using System.Collections.Generic;

namespace ThePaperWall.Core.Downloads
{
    public class AsyncDownloadManager : IAsyncDownloadManager
    {
        public static Dictionary<string, IBitmap> cache = new Dictionary<string, IBitmap>();

        public async Task<IBitmap> DownloadImage(ImageMetaData imageMetaData, IProgress<ProgressEvent> progress = null)
        {
            //return await BlobCache.LocalMachine.LoadImageFromUrl(imageMetaData.imageUrl).GetAwaiter();
            IBitmap image = null;
            var shouldFetch = false;

            if(!cache.TryGetValue(imageMetaData.imageUrl, out image))
            {
                image = await Fetch(imageMetaData.imageUrl, progress);
                cache[imageMetaData.imageUrl] = image;
            }
          
            return image;
           
           
            //BlobCache.LocalMachine.InsertObject(imageMetaData.imageUrl, image);

           //return await BlobCache.LocalMachine.GetOrFetchObject<IBitmap>(image.imageUrl, () => Fetch(image.imageUrl, progress)).ToTask();
                             
        }

        private async Task<IBitmap> Fetch(string imageUrl, IProgress<ProgressEvent> progress = null)
        {
            using (HttpClient wc = new HttpClient())
            {
                var stream = await wc.GetStreamAsyncWithProgress(imageUrl, progress);
                return await BitmapLoader.Current.Load(stream, null, null);
            }   
        }
    }
}


