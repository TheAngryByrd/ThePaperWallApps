using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ReactiveCaliburn;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using Splat;
using ThePaperWall.Core.Models;
using System.Collections.ObjectModel;
using ThePaperWall.Core.Framework;

namespace ThePaperWall.WinRT.ViewModels
{
    public class CategoryListViewModel : BaseReactiveScreen
    {
        private readonly IThemeService themeService;
        private readonly IRssReader rssReader;
        private readonly IAsyncDownloadManager downloadManager;
        private readonly INavigationService navigationService;

        public CategoryListViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager,
            INavigationService navigationService)
        {
            this.themeService = themeService;
            this.rssReader = rssReader;
            this.downloadManager = downloadManager;
            this.navigationService = navigationService;
        }

        public string Category { get; set; }
        public ObservableCollection<CategoryItem> _categoryItems = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> CategoryItems
        {
            get { return _categoryItems; }
        }
        protected override async Task OnActivate()
        {
            var theme = themeService.GetThemes().Categories.First(c => c.Name == Category);
            var feed = await rssReader.GetFeed(theme.FeedUrl);
            var images = rssReader.GetImageMetaData(feed).ToObservable();
            images.Subscribe(CreateCategoryItem);
        }

        private void CreateCategoryItem(ImageMetaData imageMetaData)
        {
            Func<Task<IBitmap>> lazyImageFactory = () => downloadManager.DownloadImage(imageMetaData.imageThumbnail);
            Execute.BeginOnUIThread((() => CategoryItems.Add(new CategoryItem(imageMetaData.Category, lazyImageFactory))));
        }

        
        
    }
}
