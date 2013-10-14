﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Caliburn.Micro;
using LinqToAwait;
using ReactiveCaliburn;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using ThePaperWall.WinRT.Common;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

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
        }

        private Themes _themes;

        protected override async Task OnActivate()
        {
            _themes = _themeService.GetThemes(WallpaperResource.Feeds);
            GetWallpaperOfTheDay();
            GetTop4WallPaperItems();
            GetCategoryItems();
        }
  
        private async void GetCategoryItems()
        {
            var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            await _themes.Categories.OrderBy(c => c.Name).AsAsync()
                .ForEachAsync(async c=>
                {                    
                    var feed = await _rssReader.GetFeed(c.FeedUrl);
                    var firstImageFromFeed = _rssReader.GetImageMetaData(feed).First();
                    firstImageFromFeed.Category = c.Name;
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => CategoryItems.Add(new CategoryItem(_downloadManager, firstImageFromFeed)));
                    
                }); 
        }
  
        private async void GetTop4WallPaperItems()
        {          
            var rssForFeed = await _rssReader.GetFeed(_themes.Top4.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed);

            foreach(var imd in imageMetaData)
            {
                Top4Items.Add(new CategoryItem(_downloadManager,imd));
            }   
        }
  
        private void GetWallpaperOfTheDay()
        {
            System.Action getMainHeroImage =async () =>
            {
                var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
                var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();

                MainImage = (await _downloadManager.DownloadImage(imageMetaData.imageUrl)).ToNative();
            };
            getMainHeroImage.OnUIThreadAsync(); 
        }

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public ObservableCollection<CategoryItem> _top4Items = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> Top4Items
        {
            get { return _top4Items; }
        }    
        
        public ObservableCollection<CategoryItem> _categoryItems = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> CategoryItems
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

    public class CategoryItem : ReactiveObject
    {

        public CategoryItem(IAsyncDownloadManager downloaderManager, ImageMetaData imageMetaData)
        {
            Category = imageMetaData.Category;
            System.Action lazyImage = async () => 
                {
                    ImagePath = (await downloaderManager.DownloadImage(imageMetaData.imageUrl)).ToNative();
                };
            lazyImage.BeginOnUIThread();
        }

        public string Category { get; set; }
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
    }
}
