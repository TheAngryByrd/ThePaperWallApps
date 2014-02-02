using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Splat;
using ThePaperWall.Core.Downloads;
using Microsoft.Phone.Shell;
using ThePaperWall.Core.Models;
using System.Net.Http;

namespace ThePaperWall.WP8.Services
{
    public class NotificationService : INotificationService
    {
        public NotificationService(IAsyncDownloadManager downloadManager)
        {
            this.downloadManager = downloadManager;
        }

        private readonly IAsyncDownloadManager downloadManager;

        private IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

        public async Task CreateLiveTileFromImageMetadata(List<ImageMetaData> images)
        {
            var localImages = await DownloadImages(images.Select(x => x.GetResizedImageUrl(691,366)));
            UpdateTiles(localImages);
        }

        private async Task<IEnumerable<Uri>> DownloadImages(IEnumerable<string> imageUrls)
        {
            var imagePaths = new List<string>();
            List<byte[]> images = await imageUrls.ToObservable()
                                        .SelectMany(async x => await downloadManager.Download(x))
                                        .Aggregate(new List<byte[]>(), (l, x) =>
                                        {
                                            l.Add(x);
                                            return l;
                                        });
            
         
          
            var localI = 0;
            foreach(var image in images)
            {
                var imagePath = "shared/shellcontent/myImage" + localI + ".jpg";
                imagePaths.Add(imagePath);
                using (var isoStoreFile = isoStore.OpenFile(imagePath,
                                                                FileMode.Create,
                                                                FileAccess.ReadWrite))
                                     
                await isoStoreFile.WriteAsync(image, 0, image.Length);                                        
                    
                localI ++;
            }         
            
            return imagePaths.Select(x =>new Uri(string.Format("isostore:/{0}",x),UriKind.Absolute));
        }

        private void UpdateTiles(IEnumerable<Uri> localImagePaths)
        {          

            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault();
   
            if (TileToFind != null)
            {
                CycleTileData oCycleicon = new CycleTileData();
                oCycleicon.CycleImages = localImagePaths;
                TileToFind.Update(oCycleicon);
            }
          
        }
    }
}
