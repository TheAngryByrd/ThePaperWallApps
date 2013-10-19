using System;
using Caliburn.Micro;
using ReactiveUI;
using Splat;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

namespace ThePaperWall.WinRT.ViewModels
{
    public class CategoryItem : ReactiveObject, IComparable<CategoryItem>
    {
        public CategoryItem(string name, Func<Task<IBitmap>> lazyImageFactory)
        {
            Name = name;
            LoadImage(lazyImageFactory);
        }

        private async void LoadImage(Func<Task<IBitmap>> lazyImageFactory)
        {
            var image = await lazyImageFactory();
            Execute.BeginOnUIThread(() => ImagePath = image.ToNative());
        }

        public string Name { get; set; }
 
        public ImageSource _imagePath;
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

        public int CompareTo(CategoryItem other)
        {
            return string.Compare(this.Name, other.Name);
        }

    }
}