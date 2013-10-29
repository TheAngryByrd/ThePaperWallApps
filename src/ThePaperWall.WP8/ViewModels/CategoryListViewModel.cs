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


        private bool _progressBarIsVisible = true;
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


        protected override async Task OnActivate()
        {
            ProgressBarIsVisible = true;
            await GetImages();
            ProgressBarIsVisible = false;

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

        private async Task GetImages()
        {
            try
            {
                var theme = _themeService.GetThemes().Categories.First(c => c.Name == Category);
                var feed = await _rssReader.GetFeed(theme.FeedUrl);
                var images = _rssReader.GetImageMetaData(feed).Take(10);
                await Task.WhenAll(images.Select(x => CreateCategoryItem(x)));
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
