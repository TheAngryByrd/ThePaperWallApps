using Akavache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ThePaperWall.Core.Models;

namespace ThePaperWall.WP8.Helpers
{
    public class DownloadHelper : IDownloadHelper
    {
        public async Task<BitmapImage> GetImage(ImageMetaData imageMetaData, bool go = false)
        {
            byte[] imageBytes = null;
            bool shouldGet = false;
            string url = go ? imageMetaData.imageThumbnail : imageMetaData.imageUrl;
            try
            {

                imageBytes = await BlobCache.LocalMachine.GetAsync(url);
            }
            catch (Exception e)
            {
                shouldGet = true;
            }
            if (shouldGet)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        imageBytes = await client.GetByteArrayAsync(url);
                        await BlobCache.LocalMachine.Insert(url, imageBytes);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            var imageStream = new MemoryStream(imageBytes);

            //BECAUSE WP8 SAID SO
            BitmapImage image = new BitmapImage();
            image.SetSource(imageStream);
            return image;
        }
    }
}
