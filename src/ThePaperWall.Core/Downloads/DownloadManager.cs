using System.Reactive.Linq;
using Akavache;
using Splat;
using System;
using System.Net.Http;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ThePaperWall.Core.Models;
using System.IO;

namespace ThePaperWall.Core.Downloads
{
    public class AsyncDownloadManager : IAsyncDownloadManager
    {
        public async Task<IBitmap> DownloadImage(ImageMetaData imageMetaData, IProgress<ProgressEvent> progress = null)
        {
            return await BlobCache.LocalMachine.LoadImageFromUrl(imageMetaData.imageUrl);
            //Stream imageStream = null;
            //var shouldFetch = false;
            //try
            //{
            //    imageStream = await BlobCache.LocalMachine.GetObjectAsync<Stream>(imageMetaData.imageUrl);
            //}catch(Exception _)
            //{
            //    shouldFetch = true;
            //}
            //if(shouldFetch)           
            //{
            //    imageStream = await Fetch(imageMetaData.imageUrl, progress);
            //    await BlobCache.LocalMachine.InsertObject(imageMetaData.imageUrl, imageStream);
            //}

            //return await BitmapLoader.Current.Load(imageStream, null, null);
         
           
           
            //BlobCache.LocalMachine.InsertObject(imageMetaData.imageUrl, image);

           //return await BlobCache.LocalMachine.GetOrFetchObject<IBitmap>(image.imageUrl, () => Fetch(image.imageUrl, progress)).ToTask();
                             
        }

        private async Task<Stream> Fetch(string imageUrl, IProgress<ProgressEvent> progress = null)
        {
            using (HttpClient wc = new HttpClient())
            {
                return await wc.GetStreamAsyncWithProgress(imageUrl, progress);
              
            }   
        }
    }
}


