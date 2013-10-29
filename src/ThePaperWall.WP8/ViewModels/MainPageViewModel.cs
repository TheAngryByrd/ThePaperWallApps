using System;
using System.Linq;
using Caliburn.Micro;
using ReactiveCaliburn;
using System.Threading.Tasks;
using ReactiveUI;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using Splat;
using System.Windows.Media;
using ThePaperWall.WP8.Helpers;
using Windows.UI.Core;
using ThePaperWall.WP8.Views;
using System.IO;
using System.Windows.Media.Imaging;
using System.Net;
using System.Net.Http;
using Akavache;
using System.Reactive.Linq;
using ThePaperWall.Core.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Phone.System.UserProfile;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Phone.Net.NetworkInformation;

namespace ThePaperWall.WP8.ViewModels
{
    public class MainPageViewModel : ReactiveScreen
    {
        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;
        private readonly INavigationService _navigationService;
        private readonly ILockscreenHelper _lockscreenHelper;

        public MainPageViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager,
            INavigationService navigationService,
            ILockscreenHelper lockscreenHelper)
        {
            _themeService = themeService;
            _rssReader = rssReader;
            _downloadManager = downloadManager;
            _navigationService = navigationService;
            _lockscreenHelper = lockscreenHelper;


            this.ObservableForProperty(vm => vm.SelectedCategory).Select(_ => _.GetValue()).Subscribe(NavigateToCategoryList);
            this.ObservableForProperty(vm => vm.SelectedTop4).Select(_ => _.GetValue()).Where(v => v != null).Subscribe(Top4Selected);
        }

        private Themes _themes;
        protected override async Task OnActivate()
        {
            
            var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            if (!isNetworkAvailable)
            {
                MessageBox.Show("Please check your network connection");
                return;
            }

            _themes = _themeService.GetThemes(WallpaperResource.Feeds);
            var t1=  GetWallpaperOfTheDay();

            var t2 = GetTop4WallPaperItems();
            var t3 = GetCategoryItems();

            await Task.WhenAll(t1,t2,t3);
            ProgressBarIsVisible = false;
        }
  
        private void Top4Selected(CategoryItem item)
        {
            _lockscreenHelper.SetLockscreen(item.Id);
            SelectedTop4 = null;
        }

        private void NavigateToCategoryList(CategoryItem item)
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

        protected override async Task OnDeactivate(bool close)
        {
            Top2Items.Clear();
            Bottom2Items.Clear();
            CategoryItems.Clear();
        }

    
        

        public ObservableCollection<CategoryItem> _top2Items = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> Top2Items
        {
            get { return _top2Items; }
        }

        public ObservableCollection<CategoryItem> _botton2Items = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> Bottom2Items
        {
            get { return _botton2Items; }
        }

        private CategoryItem _selectedCategory;
        public CategoryItem SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCategory, value);
            }
        }
        private CategoryItem _selectedTop4;
        public CategoryItem SelectedTop4
        {
            get
            {
                return _selectedTop4;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTop4, value);
            }
        }

        private async Task GetTop4WallPaperItems()
        {
            rss rssForFeed = null;
            try
            {
                rssForFeed = await _rssReader.GetFeed(_themes.Top4.FeedUrl);
                var imageMetaData = _rssReader.GetImageMetaData(rssForFeed);
                var taskList = new List<Task>();
                foreach (var imd in imageMetaData.Skip(0).Take(2))
                {
                    Func<Task<IBitmap>> lazyImageFactory = () => _downloadManager.DownloadImage(imd.imageThumbnail);
                    var categoryItem = new CategoryItem(imd.imageUrl, imd.Category, lazyImageFactory);
                    Top2Items.Add(categoryItem);
                    taskList.Add(categoryItem.LoadImage());
                }

                foreach (var imd in imageMetaData.Skip(2).Take(2))
                {
                    Func<Task<IBitmap>> lazyImageFactory = () => _downloadManager.DownloadImage(imd.imageThumbnail);
                    var categoryItem = new CategoryItem(imd.imageUrl, imd.Category, lazyImageFactory);
                    Bottom2Items.Add(categoryItem);
                    taskList.Add(categoryItem.LoadImage());
                }
                await Task.WhenAll(taskList);
            }
            catch (Exception )
            {

            }
         
          
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

        public SortableObservableCollection<CategoryItem> _categoryItems = new SortableObservableCollection<CategoryItem>();
        public SortableObservableCollection<CategoryItem> CategoryItems
        {
            get { return _categoryItems; }
        }   

        private async Task GetCategoryItems()
        {
            await Task.WhenAll(_themes.All.Select(x => GetCategory(x)));
        }

        private async Task GetCategory(Theme theme)
        {
            try
            {
                //var feed = await _rssReader.GetFeed();
                //var firstImageFromFeed = _rssReader.GetImageMetaData(feed).First();
               // firstImageFromFeed.Category = theme.Name;

                await Execute.OnUIThreadAsync(async () =>
                {
                    var categoryItem = new CategoryItem(theme.FeedUrl, theme.Name);
                    CategoryItems.Add(categoryItem);
                });
            }
            catch (Exception e)
            {
                if (!shown)
                {
                    shown = true;
                    //MessageBox.Show("Please check your network connection");
                }
                
            }

        }
        bool shown = false;
        private MainPageView _view2;

        protected internal override async Task OnViewAttached(object view, object context)
        {
            _view2 = view as MainPageView;
        }


        private async Task GetWallpaperOfTheDay()
        {
            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();
            
            try
            {
                var image = await BlobCache.LocalMachine.LoadImageFromUrl(imageMetaData.imageUrl, absoluteExpiration: DateTimeOffset.Now.AddSeconds(30));

                //var image = await _downloadHelper.GetImage(imageMetaData);
                ImageBrush brush = new ImageBrush
                {
                    Opacity = 0.4,
                    Stretch = Stretch.Fill,
                    ImageSource = image.ToNative()
                };
            
                Execute.BeginOnUIThread(() =>
                {
                    _view2.Panorama.Background = brush;
                });
            }
            catch (Exception e)
            {
            }
        }

        private IBitmap _wallpaper;
        private ImageSource _wallpaperOfTheDay;
        public ImageSource WallpaperOfTheDay
        {
            get { return _wallpaperOfTheDay; }
            set { this.RaiseAndSetIfChanged(ref _wallpaperOfTheDay, value); }
        }
    }
}