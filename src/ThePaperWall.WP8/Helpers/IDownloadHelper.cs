using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Akavache;
using ThePaperWall.Core.Models;

namespace ThePaperWall.WP8.Helpers
{
    public interface IDownloadHelper
    {
        Task<BitmapImage> GetImage(ImageMetaData imageMetaData, bool go = false);
    }
}