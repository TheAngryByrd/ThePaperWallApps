using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using Caliburn.Micro;
using ReactiveCaliburn;
using ReactiveUI;
using Splat;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using ThePaperWall.WinRT.Common;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Akavache;
using Punchclock;
using System.Reactive;
using System.Collections.Generic;
using ThePaperWall.Core.Framework;
using Windows.ApplicationModel.Core;
using Windows.Storage.Streams;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ThePaperWall.WinRT.ViewModels
{
    public class HubViewModel : ReactiveScreen
    {
        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;

        public HubViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager)
        {
            _themeService = themeService;
            _rssReader = rssReader;
            _downloadManager = downloadManager;
            //BlobCache.LocalMachine.Dispose();
        }

        private Themes _themes;


        protected override async Task OnActivate()
        {
            _dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            _themes = _themeService.GetThemes(WallpaperResource.Feeds);
            GetWallpaperOfTheDay();
            GetTop4WallPaperItems();       
            GetCategoryItems();
        }
  
        private CoreDispatcher _dispatcher;

        private void GetCategoryItems()
        {

            _themes.Categories
                         .ToObservable()            
                         .Subscribe(GetCategory); 
        }

        private async void GetCategory(Theme theme)
        {
            var feed = await _rssReader.GetFeed(theme.FeedUrl);
            var firstImageFromFeed = _rssReader.GetImageMetaData(feed).First();
            firstImageFromFeed.Category = theme.Name;
            Func<Task<IBitmap>> lazyImageFactory = () => _downloadManager.DownloadImage(firstImageFromFeed.imageThumbnail);

            Execute.BeginOnUIThread((() => CategoryItems.Add(new CategoryItem(firstImageFromFeed.Category,lazyImageFactory))));
        }
  
        private async void GetTop4WallPaperItems()
        {          
            var rssForFeed = await _rssReader.GetFeed(_themes.Top4.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed);

            foreach(var imd in imageMetaData)
            {
                Func<Task<IBitmap>> lazyImageFactory = () => _downloadManager.DownloadImage(imd.imageThumbnail);
                Top4Items.Add(new CategoryItem(imd.Category, lazyImageFactory));
            }   
        }
  
        private async void GetWallpaperOfTheDay()
        {           
            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();
           
            var lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);
            var imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);


            await Execute.OnUIThreadAsync(async () => WallpaperOfTheDay = (await lowResImageTask).ToNative());
          _dispatcher.RunAsync(CoreDispatcherPriority.High, async () => WallpaperOfTheDay = (await imageTask).ToNative());
            //Execute.BeginOnUIThread(async () => WallpaperOfTheDay = (await imageTask).ToNative());
        }


        public SortableObservableCollection<CategoryItem> _top4Items = new SortableObservableCollection<CategoryItem>();
        public SortableObservableCollection<CategoryItem> Top4Items
        {
            get { return _top4Items; }
        }

        public SortableObservableCollection<CategoryItem> _categoryItems = new SortableObservableCollection<CategoryItem>();
        public SortableObservableCollection<CategoryItem> CategoryItems
        {
            get { return _categoryItems; }
        }

        private ImageSource _wallpaperOfTheDay;
        public ImageSource WallpaperOfTheDay
        {
            get { return _wallpaperOfTheDay; }
            set { this.RaiseAndSetIfChanged(ref _wallpaperOfTheDay, value); }
        }
    }  
    
}
