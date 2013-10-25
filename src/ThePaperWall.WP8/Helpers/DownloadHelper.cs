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
using Punchclock;
using ThePaperWall.Core.Models;
using System.Windows;

namespace ThePaperWall.WP8.Helpers
{
    public class DownloadHelper : IDownloadHelper
    {
        private static OperationQueue queue = new OperationQueue(2);
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
                imageBytes = await queue.Enqueue(1,async () => 
                    {

                        using (var client = new HttpClient())
                        {
                            byte[] tempimageBytes = null;
                            try
                            {
                                tempimageBytes = await client.GetByteArrayAsync(url);
                                await BlobCache.LocalMachine.Insert(url, tempimageBytes);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("Please check your network connection");
                            }
                            return tempimageBytes;
                        }
                    });
            }
            var imageStream = new MemoryStream(imageBytes);

            //BECAUSE WP8 SAID SO
            BitmapImage image = new BitmapImage();
            image.SetSource(imageStream);
            return image;
        }
    }
}
