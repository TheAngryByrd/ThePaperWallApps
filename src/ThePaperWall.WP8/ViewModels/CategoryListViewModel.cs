using System.Reactive.Linq;
using System.Threading;
using Caliburn.Micro;
using ReactiveCaliburn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using ThePaperWall.WP8.Helpers;
using Splat;
using ThePaperWall.ViewModels;

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
            _themeService = themeService;
            _rssReader = rssReader;
            _downloadManager = downloadManager;
            _navigationService = navigationService;
            _lockscreen = lockscreen;


            AddMorePicturesCommand = Items.CountChanged
                                          .Select(_ =>  Items.Count < imageMetaData.Count())
                                          .ToCommand();

            AddMorePicturesCommand
                .RegisterAsyncTask(value => GetImages((int)value));

            FullScreenCommand = new ReactiveCommand();
            SetLockScreenCommand = new ReactiveCommand();
            DownloadImageCommand = new ReactiveCommand();

        }

        public ReactiveCommand AddMorePicturesCommand { get; set; }
        public ReactiveCommand FullScreenCommand { get; set; } 
        public ReactiveCommand SetLockScreenCommand { get; set; } 
        public ReactiveCommand DownloadImageCommand { get; set; } 
        
        protected override async Task OnInitialize()
        {        
            var theme = _themeService.GetThemes().All.First(c => c.Name == Category);
            var feed = await _rssReader.GetFeed(theme.FeedUrl);
            imageMetaData = _rssReader.GetImageMetaData(feed);  
        }

        private IEnumerable<ImageMetaData> imageMetaData;        

        private async Task GetImages(int skip)
        {
            try
            {             
                var task = Task.WhenAll(imageMetaData.Skip(Items.Count).Take(skip).Select(x => CreateCategoryItem(x)));                
                await task;                
            }
            catch (Exception e)
            {
            }          
        }
        private async Task CreateCategoryItem(ImageMetaData imageMetaData)
        {
            Func<Task<IBitmap>> lazyImageFactory = () => _downloadManager.DownloadImage(imageMetaData.imageThumbnail);
            var category = new CategoryItem(imageMetaData.imageUrl, imageMetaData.Category, lazyImageFactory);
          
            await category.LoadImage();
            Items.Add(category);
        }

        public ReactiveList<CategoryItem> _categoryItems = new ReactiveList<CategoryItem>();
        public ReactiveList<CategoryItem> Items
        {
            get { return _categoryItems; }
        }

        public async void SetLockscreen(CategoryItem item)
        {           
            await _lockscreen.SetLockscreen(item.Id);
        }

    }
}
