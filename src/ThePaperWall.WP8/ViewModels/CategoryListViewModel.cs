using Caliburn.Micro;
using ReactiveCaliburn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ReactiveUI;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using ThePaperWall.WP8.Helpers;
using Splat;
using System.Reactive.Linq;

namespace ThePaperWall.WP8.ViewModels
{
    public class CategoryListViewModel : ReactiveScreen
    {
        public string Category { get; set; }

        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;
        private readonly INavigationService _navigationService;


        private readonly ILockscreenHelper _lockscreen;

        public CategoryListViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager,
            INavigationService navigationService,
            ILockscreenHelper lockscreen)
        {
            this._themeService = themeService;
            this._rssReader = rssReader;
            this._downloadManager = downloadManager;
            this._navigationService = navigationService;
            _lockscreen = lockscreen;
        
        }


        private bool _progressBarIsVisible = false;

        public bool ProgressBarIsVisible
        {
            get
            {
                return _progressBarIsVisible;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _progressBarIsVisible, value);
            }
        }

        protected override async Task OnDeactivate(bool close)
        {
            try
            {
                Items.Clear();
            }
            catch (Exception e)
            {
            }
        }

        private IEnumerable<ImageMetaData> allImages;

        private int imageCount = 0;
        private int refcount = 0;
        public async Task AddMorePictures(int skip)
        {           
            ProgressBarIsVisible = true;    
            refcount++;
            await GetImages(skip);
            refcount--;
            if(refcount == 0)
                ProgressBarIsVisible = false;
        }

        private async Task GetImages(int skip)
        {
            try
            {
                var theme = _themeService.GetThemes().Categories.First(c => c.Name == Category);
                var feed = await _rssReader.GetFeed(theme.FeedUrl);
                allImages = allImages ??_rssReader.GetImageMetaData(feed);
                if(imageCount <allImages.Count())
                {
                    var task = Task.WhenAll(allImages.Skip(imageCount).Take(skip).Select(x => CreateCategoryItem(x)));
                    imageCount += skip;
                    await task;
                }
            }
            catch (Exception e)
            {
            }          
        }
        private async Task CreateCategoryItem(ImageMetaData imageMetaData)
        {

            Func<Task<IBitmap>> lazyImageFactory = () => _downloadManager.DownloadImage(imageMetaData.imageThumbnail);
            var category = new CategoryItem(imageMetaData.imageUrl, imageMetaData.Category, lazyImageFactory);
            Items.Add(category);
            await category.LoadImage();
        }

        public ObservableCollection<CategoryItem> _categoryItems = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> Items
        {
            get { return _categoryItems; }
        }

        public async void SetLockscreen(CategoryItem item)
        {
            ProgressBarIsVisible = true;
            await _lockscreen.SetLockscreen(item.Id);
            ProgressBarIsVisible = false;
        }

    }
}
