using System;
using Caliburn.Micro;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Models;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

namespace ThePaperWall.WinRT.ViewModels
{
    public class CategoryItem : ReactiveObject, IComparable<CategoryItem>
    {
        public CategoryItem(IAsyncDownloadManager downloaderManager,
            ImageMetaData imageMetaData, int priority = 1)
        {
            Category = imageMetaData.Category;
            LazyLoadImage(downloaderManager, imageMetaData, priority);
        }
  
        private async void LazyLoadImage(IAsyncDownloadManager downloaderManager, ImageMetaData imageMetaData, int priority)
        {
            var image = (await downloaderManager.DownloadImage(imageMetaData.imageThumbnail, priority:priority));
            Execute.BeginOnUIThread(() => ImagePath = image.ToNative());
        }



        public string Category { get; set; }

        public ImageSource _imagePath;

        public int CompareTo(CategoryItem other)
        {
            return string.Compare(this.Category, other.Category);
        }

        public ImageSource ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _imagePath, value);
            }
        }
    }
}