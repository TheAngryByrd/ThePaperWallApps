using Caliburn.Micro;
using ReactiveCaliburn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using ThePaperWall.WP8.Helpers;

namespace ThePaperWall.WP8.ViewModels
{
    public class CategoryListViewModel : ReactiveScreen
    {
        public string Category { get; set; }

        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;
        private readonly INavigationService _navigationService;

        private readonly IDownloadHelper _downloadHelper;

        public CategoryListViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager,
            INavigationService navigationService,
            IDownloadHelper downloadHelper)
        {
            this._themeService = themeService;
            this._rssReader = rssReader;
            this._downloadManager = downloadManager;
            this._navigationService = navigationService;
            _downloadHelper = downloadHelper;
        
        }

        protected override async Task OnActivate()
        {
            await GetImages();
        }

        private async Task GetImages()
        {
            var theme = _themeService.GetThemes().Categories.First(c => c.Name == Category);
            var feed = await _rssReader.GetFeed(theme.FeedUrl);
            var images = _rssReader.GetImageMetaData(feed).Take(10);
            await Task.WhenAll(images.Select(x => CreateCategoryItem(x)));
           // ProgressBarIsVisible = false;
        }
        private async Task CreateCategoryItem(ImageMetaData imageMetaData)
        {
            Func<Task<BitmapImage>> lazyImageFactory = () => _downloadHelper.GetImage(imageMetaData,true);
            var category = new CategoryItem(imageMetaData.imageUrl, imageMetaData.Category, lazyImageFactory);
            Items.Add(category);
            await category.LoadImage();
        }

        public ObservableCollection<CategoryItem> _categoryItems = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> Items
        {
            get { return _categoryItems; }
        }   

    }
}
