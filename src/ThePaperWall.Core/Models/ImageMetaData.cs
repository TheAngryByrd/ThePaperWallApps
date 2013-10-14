using System;
using System.IO;

namespace ThePaperWall.Core.Models
{
    public class ImageMetaData
    {
        public string imageThumbnail { get; set; }

        public string imageUrl
        {
            get
            {
                var bigImage = imageThumbnail.Replace("small", "big");
                return bigImage.Substring(0, bigImage.IndexOf("?"));
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
    }
}
