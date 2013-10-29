//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Reactive.Linq;
//using Caliburn.Micro;
//using ReactiveCaliburn;
//using ReactiveUI;
//using Splat;
//using ThePaperWall.Core.Downloads;
//using ThePaperWall.Core.Feeds;
//using ThePaperWall.Core.Models;
//using ThePaperWall.Core.Rss;
//using ThePaperWall.Core.Framework;

//namespace ThePaperWall.ViewModels
//{
//    public class MainPageViewModelBase : ReactiveScreen
//    {
//        private readonly IThemeService _themeService;
//        private readonly IRssReader _rssReader;
//        private readonly IAsyncDownloadManager _downloadManager;

//        public MainPageViewModelBase(IThemeService themeService,
//            IRssReader rssReader,
//            IAsyncDownloadManager downloadManager)
//        {
//            _themeService = themeService;
//            _rssReader = rssReader;
//            _downloadManager = downloadManager;
//        }

//        private Themes _themes;

//        protected override async Task OnActivate()
//        {
//            _themes = _themeService.GetThemes(WallpaperResource.Feeds);
//            GetWallpaperOfTheDay();
//            GetTop4WallPaperItems();
//            GetCategoryItems();
//        }

//        private void GetCategoryItems()
//        {
//            _themes.Categories
//                   .ToObservable()
//                   .Subscribe(GetCategory);
//        }

//        private async void GetCategory(Theme theme)
//        {
//            var feed = await _rssReader.GetFeed(theme.FeedUrl);
//            var firstImageFromFeed = _rssReader.GetImageMetaData(feed).First();
//            firstImageFromFeed.Category = theme.Name;
//            Execute.BeginOnUIThread((() => CategoryItems.Add(new CategoryItem(_downloadManager, firstImageFromFeed))));
//        }

//        private async void GetTop4WallPaperItems()
//        {
//            var rssForFeed = await _rssReader.GetFeed(_themes.Top4.FeedUrl);
//            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed);

//            foreach (var imd in imageMetaData)
//            {
//                Top4Items.Add(new CategoryItem(_downloadManager, imd, 2));
//            }
//        }

//        private async Task<Task<IBitmap>[]> GetWallpaperOfTheDay()
//        {
//            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
//            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();
//            var imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);
//            var lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);
//            return new[] { imageTask, lowResImageTask };
//            //await Execute.OnUIThreadAsync(async () => MainImage = (await lowResImageTask).ToNative());
//            //Execute.BeginOnUIThread(async () => MainImage = (await imageTask).ToNative());
//        }

//        public SortableObservableCollection<CategoryItem> _top4Items = new SortableObservableCollection<CategoryItem>();

//        public SortableObservableCollection<CategoryItem> Top4Items
//        {
//            get { return _top4Items; }
//        }

//        public SortableObservableCollection<CategoryItem> _categoryItems = new SortableObservableCollection<CategoryItem>();

//        public SortableObservableCollection<CategoryItem> CategoryItems
//        {
//            get { return _categoryItems; }
//        }

 
//    }
//}