using Splat;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ThePaperWall.Core.Models;
using System.Collections.Generic;
using System.IO;
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
            //IBitmap image = null;
            //var shouldFetch = false;

            //if(!cache.TryGetValue(imageUrl, out image))
            //{
            //    image = await Fetch(imageUrl, progress);
            //    cache[imageUrl] = image;
            //}
          
            //return image;
        }

        private async Task<IBitmap> Fetch(string imageUrl, IProgress<ProgressEvent> progress = null)
        {
            using (HttpClient wc = new HttpClient())
            {
                var stream = await wc.GetStreamAsync(imageUrl);
                          
                byte[] b;
                using (var br = new BinaryReader(stream)) {
                  b = br.ReadBytes((int)stream.Length);
                }
                MemoryStream ms = new MemoryStream(b);
                //var stream = await wc.GetStreamAsyncWithProgress(imageUrl, progress);
                return await BitmapLoader.Current.Load(ms, null, null);
            }   
        }
    }
}


