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
            BlobCache.LocalMachine.Dispose();
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
            Execute.BeginOnUIThread((() => CategoryItems.Add(new CategoryItem(_downloadManager, firstImageFromFeed))));
        }
  
        private async void GetTop4WallPaperItems()
        {          
            var rssForFeed = await _rssReader.GetFeed(_themes.Top4.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed);

            foreach(var imd in imageMetaData)
            {
                Top4Items.Add(new CategoryItem(_downloadManager,imd,2));
            }   
        }
  
        private async void GetWallpaperOfTheDay()
        {           
            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();
            var imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);
            var lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);           
          

            await Execute.OnUIThreadAsync(async () => MainImage = (await lowResImageTask).ToNative());
            Execute.BeginOnUIThread(async () => MainImage = (await imageTask).ToNative());
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

        private ImageSource _mainImage;
        public ImageSource MainImage
        {
            get { return _mainImage; }
            set { this.RaiseAndSetIfChanged(ref _mainImage, value); }
        }
    }  
    
}
