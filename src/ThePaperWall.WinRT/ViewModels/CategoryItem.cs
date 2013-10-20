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
            _lazyImageFactory = lazyImageFactory;
            //LoadImage(lazyImageFactory);
        }

        public async Task LoadImage()
        {
            var image = await _lazyImageFactory();
            await Execute.OnUIThreadAsync(() => ImagePath = image.ToNative());
        }

        public string Name { get; set; }
 
        private readonly Func<Task<IBitmap>> _lazyImageFactory;

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