using Caliburn.Micro;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThePaperWall.WP8.ViewModels
{
    public class CategoryItem : ReactiveObject, IComparable<CategoryItem>
    {

        public CategoryItem(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public CategoryItem(string id, string name, Func<Task<IBitmap>> lazyImageFactory = null)
        {
            Id = id;
            Name = name;
            _lazyImageFactory = lazyImageFactory;
        }

        public CategoryItem(string id, string name, Func<Task<BitmapImage>> lazyImageFactory = null)
        {
            Id = id;
            Name = name;
            _lazyImageFactory2 = lazyImageFactory;
        }

        private BitmapImage _image;

        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        public async Task LoadImage()
        {
            try
            {
                if (_lazyImageFactory != null)
                {
                    var image = await _lazyImageFactory();
                    await Execute.OnUIThreadAsync(() => ImagePath = image.ToNative());
                }
                if (_lazyImageFactory2 != null)
                {
                    _image = await _lazyImageFactory2();
                    await Execute.OnUIThreadAsync(() => ImagePath = _image);
                }
            }
            catch (Exception e)
            {
            }
        }

        public string Id { get; private set; }

        public string Name { get; set; }

        private readonly Func<Task<IBitmap>> _lazyImageFactory;
        private Func<Task<BitmapImage>> _lazyImageFactory2;

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
