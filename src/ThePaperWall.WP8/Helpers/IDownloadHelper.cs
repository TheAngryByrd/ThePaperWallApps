using System;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Akavache;
using Punchclock;
using ThePaperWall.Core.Models;

namespace ThePaperWall.WP8.Helpers
{
    public interface IDownloadHelper
    {
        Task<Stream> GetImageStream(ImageMetaData imageMetaData, bool getThumbnail = false);

        Task<BitmapImage> GetImage(ImageMetaData imageMetaData, bool go = false);
    }
}