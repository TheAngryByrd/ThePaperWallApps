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
            this._themeService = themeService;
            this._rssReader = rssReader;
            this._downloadManager = downloadManager;
            this._navigationService = navigationService;
            _lockscreen = lockscreen;
            
            AddMorePicturesCommand = new ReactiveCommand();
            AddMorePicturesCommand
                .RegisterAsyncTask(value => AddMorePictures((int)value));
        }

        public ReactiveCommand AddMorePicturesCommand { get; set; }
        public ReactiveCommand FullScreenCommand { get; set; } 
        public ReactiveCommand SetLockScreenCommand { get; set; } 
        public ReactiveCommand DownloadImageCommand { get; set; } 

        protected override async Task OnDeactivate(bool close)
        {
            try
            {
                //Items.Clear();
            }
            catch (Exception e)
            {
            }
        }

        private IEnumerable<ImageMetaData> allImages;

        private int imageCount = 0;
        private int refCount = 0;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public async Task AddMorePictures(int skip)
        {
            if (refCount == 2)
            {
                return;
            }
            refCount++;
            //await Task.Delay(500);  
            await semaphore.WaitAsync();
            await GetImages(skip);
            semaphore.Release();
            refCount--;      
        }

        private async Task GetImages(int skip)
        {
            try
            {
                var theme = _themeService.GetThemes().All.First(c => c.Name == Category);
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
          
            await category.LoadImage();
            Items.Add(category);
        }

        public ObservableCollection<CategoryItem> _categoryItems = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> Items
        {
            get { return _categoryItems; }
        }

        public async void SetLockscreen(CategoryItem item)
        {           
            await _lockscreen.SetLockscreen(item.Id);
        }

    }
}
