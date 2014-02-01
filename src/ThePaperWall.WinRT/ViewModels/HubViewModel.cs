using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using NotificationsExtensions.TileContent;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media;
using ThePaperWall.Core.Framework;
using System.Collections.Generic;
using ThePaperWall.ViewModels;
using Windows.Data.Xml.Dom;

namespace ThePaperWall.WinRT.ViewModels
{
    public class HubViewModel : BaseReactiveScreen
    {
        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;
        private readonly INavigationService _navigationService;

        public ReactiveCommand WallpaperOfTheDayCommand { get; private set; }
        public ReactiveCommand Top4Command { get; private set; }
        public ReactiveCommand CategoryCommand { get; private set; }

        public ReactiveCommand OnActivateCommand {get; private set;}

        public HubViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager,
            INavigationService navigationService)
        {
            _themeService = themeService;
            _rssReader = rssReader;
            _downloadManager = downloadManager;
            _navigationService = navigationService;

            OnActivateCommand = new ReactiveCommand();
            OnActivateCommand.RegisterAsyncTask(_ => OnActivateWork());

            WallpaperOfTheDayCommand = new ReactiveCommand();
            WallpaperOfTheDayCommand.Subscribe(NavigateToDetailsForWallpaperOfTheDay);
            Top4Command = new ReactiveCommand();
            Top4Command.Subscribe(NavigateToDetailsForTop4Item);
            CategoryCommand = new ReactiveCommand();
            CategoryCommand.Subscribe(NavigateToCategoryList);
            //BlobCache.LocalMachine.Dispose();
            this.updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
        }



        private TileUpdater updater;


     
        private void SetupLiveTile(string imgSrc)
        {
            var square310 = TileContentFactory.CreateTileSquare310x310Image();
            square310.Image.Src = imgSrc;
            var wide = TileContentFactory.CreateTileWide310x150Image();
            wide.Image.Src = imgSrc;
            square310.Wide310x150Content = wide;
            var square150 = TileContentFactory.CreateTileSquare150x150Image();
            square150.Image.Src = imgSrc;
            wide.Square150x150Content = square150;
            var notification = square310.CreateNotification();         

            updater.Update(notification);
        }

        private void NavigateToDetailsForTop4Item(object item)
        {
            try
            {
                var categoryItem = item as CategoryItem;
                _navigationService.UriFor<ImageDetailsViewModel>()
                                  .WithParam(x => x.Category, _themes.Top4.Name)
                                  .WithParam(x => x.Id, categoryItem.Id)
                                  .Navigate();
            }
            catch (Exception e)
            {
            }
        }

        private void NavigateToDetailsForWallpaperOfTheDay(object obj)
        {
            try
            {
                _navigationService.UriFor<ImageDetailsViewModel>()
                                  .WithParam(x => x.Category, _themes.WallPaperOfTheDay.Name)
                                  .Navigate();
                
            }
            catch (Exception e)
            {
            }
        }
  
        private void NavigateToCategoryList(object item)
        {
            try
            {
                var categoryItem = item as CategoryItem;
                _navigationService.UriFor<CategoryListViewModel>()
                                  .WithParam(x => x.Category, categoryItem.Name)
                                  .Navigate();
            }
            catch (Exception e)
            {
            }
        }

        private CoreDispatcher _dispatcher;
        private Themes _themes;
        protected override async Task OnActivate()
        {
           OnActivateCommand.Execute(null);
        }
  
        private Task OnActivateWork()
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            _themes = _themeService.GetThemes(WallpaperResource.Feeds);
            var t1 = GetWallpaperOfTheDay();
            var t2 = GetTop4WallPaperItems();       
            var t3 = GetCategoryItems();
            return Task.WhenAll(t1, t2, t3);
        }

        private async Task GetWallpaperOfTheDay()
        {
            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();
            SetupLiveTile(imageMetaData.GetResizedImageUrl());
            Task<IBitmap> lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);
            Task<IBitmap> imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);

            var lowResImage = await lowResImageTask;
            await Execute.OnUIThreadAsync(async () => WallpaperOfTheDay = (lowResImage).ToNative());
            var image = await imageTask;
            await _dispatcher.RunAsync(CoreDispatcherPriority.High, async () => WallpaperOfTheDay = (image).ToNative());
        }
        private async Task GetTop4WallPaperItems()
        {
            var rssForFeed = await _rssReader.GetFeed(_themes.Top4.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed);
            var taskList = new List<Task>();
            foreach (var imd in imageMetaData)
            {
                SetupLiveTile(imd.GetResizedImageUrl());
                Func<Task<IBitmap>> lazyImageFactory = () => _downloadManager.DownloadImage(imd.imageThumbnail);
                var categoryItem = new CategoryItem(imd.imageUrl ,imd.Category, lazyImageFactory);
                Top4Items.Add(categoryItem);
                taskList.Add(categoryItem.LoadImage());
            }
            await Task.WhenAll(taskList);
        }  

        private async Task GetCategoryItems()
        {
            await Task.WhenAll(_themes.Categories.OrderBy(x => x.Name).Select(x => GetCategory(x)));                         
        }

        private async Task GetCategory(Theme theme)
        {     
       
            Func<Task<IBitmap>> lazyImageFactory = async () => 
            {
                var feed = await _rssReader.GetFeed(theme.FeedUrl);
                var firstImageFromFeed = _rssReader.GetFirstImageMetaData(feed);
                firstImageFromFeed.Category = theme.Name;
                return await _downloadManager.DownloadImage(firstImageFromFeed.imageThumbnail);
            };

            
            var categoryItem = new CategoryItem(theme.FeedUrl, theme.Name, lazyImageFactory);
            CategoryItems.Add(categoryItem);
            await categoryItem.LoadImage();
        
        }

        private ImageSource _wallpaperOfTheDay;
        public ImageSource WallpaperOfTheDay
        {
            get { return _wallpaperOfTheDay; }
            set { this.RaiseAndSetIfChanged(ref _wallpaperOfTheDay, value); }
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
    }
}
