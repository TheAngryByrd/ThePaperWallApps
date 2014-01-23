using Caliburn.Micro;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Threading.Tasks;
#if WP8
using System.Windows.Media;
#elif NETFX_CORE
using Windows.UI.Xaml.Media;
#endif

namespace ThePaperWall.ViewModels
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
        
        public async Task LoadImage()
        {
            try
            {
                if (_lazyImageFactory != null)
                {
                    var image = await _lazyImageFactory();
                    await Execute.OnUIThreadAsync(() => ImagePath = image.ToNative());
                }
            }
            catch (Exception e)
            {
            }
        }

        public string Id { get; private set; }

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
