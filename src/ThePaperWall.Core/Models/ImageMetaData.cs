using System;
using System.IO;

namespace ThePaperWall.Core.Models
{
    public class ImageMetaData
    {
        public ImageMetaData(string thumbnailUrl)
        {
            if (thumbnailUrl.IndexOf("?") >= 0)
                imageThumbnail = thumbnailUrl.Substring(0, thumbnailUrl.IndexOf("?"));
            else
                imageThumbnail = thumbnailUrl;
        }
        public string imageThumbnail { get; private set; }

        public string imageUrl
        {
            get
            {
                return imageThumbnail.Replace("small", "big");
               
            }
        }

        private string WithoutDomain
        {
            get
            {
                return imageUrl.Replace("http://thepaperwall.com",string.Empty);
            }
        }

        public string imageName { get { return GetImageFileName(imageThumbnail); } }
        public Theme Theme { get; set; }
        public Progress<double> progress { get; set; }

        public string GetImageFileName(string imageUrl)
        {
            Uri uri = new Uri(imageUrl);
            string filename = Path.GetFileName(uri.LocalPath);
            return filename;
        }

        public string Category { get; set; }

        private const string imageResizeLink = "http://www.thepaperwall.com/image.php?width={0}&height={1}&image={2}";

        public string GetResizedImageUrl(int width =310, int height = 310)
        {
            return string.Format(imageResizeLink, width,height,WithoutDomain);
        }
    }
}
