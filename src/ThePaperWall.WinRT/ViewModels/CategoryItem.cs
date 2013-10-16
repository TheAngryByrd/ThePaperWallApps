using System;
using Caliburn.Micro;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Models;
using Windows.UI.Xaml.Media;

namespace ThePaperWall.WinRT.ViewModels
{
    public class CategoryItem : ReactiveObject, IComparable<CategoryItem>
    {
        public CategoryItem(IAsyncDownloadManager downloaderManager,
            ImageMetaData imageMetaData, int priority = 1)
        {
            Category = imageMetaData.Category;
            System.Action lazyImage = async () => 
            {
                ImagePath = (await downloaderManager.DownloadImage(imageMetaData.imageUrl, priority:priority)).ToNative();
            };
            lazyImage.BeginOnUIThread();
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