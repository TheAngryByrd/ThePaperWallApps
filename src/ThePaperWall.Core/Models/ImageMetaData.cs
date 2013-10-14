using System;
using System.IO;

namespace ThePaperWall.Core.Models
{
    public class ImageMetaData
    {
        public ImageMetaData(string thumbnailUrl)
        {
            imageThumbnail = thumbnailUrl.Substring(0, thumbnailUrl.IndexOf("?"));
        }
        public string imageThumbnail { get; private set; }

        public string imageUrl
        {
            get
            {
                return imageThumbnail.Replace("small", "big");
               
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
    }
}
