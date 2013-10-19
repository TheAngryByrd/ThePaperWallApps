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
        public CategoryItem(string category, Func<Task<IBitmap>> lazyImageFactory)
        {
            Category = category;
            LoadImage(lazyImageFactory);
        }

        private async void LoadImage(Func<Task<IBitmap>> lazyImageFactory)
        {
            var imageTask = lazyImageFactory();
            var image = await imageTask;
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