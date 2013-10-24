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
using Windows.UI.Core;
using ThePaperWall.WP8.Views;
using System.IO;
using System.Windows.Media.Imaging;
using System.Net;
using System.Net.Http;
using Akavache;
using System.Reactive.Linq;
using ThePaperWall.Core.Framework;

namespace ThePaperWall.WP8.ViewModels
{
    public class MainPageViewModel : ReactiveScreen
    {
        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;

        public MainPageViewModel(IThemeService themeService,
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
            var t1=  GetWallpaperOfTheDay();

           
            //GetTop4WallPaperItems();
            var t2= GetCategoryItems();

            await Task.WhenAll(t1, t2);
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
            var feed = await _rssReader.GetFeed(theme.FeedUrl);
            var firstImageFromFeed = _rssReader.GetImageMetaData(feed).First();
            firstImageFromFeed.Category = theme.Name;

            await Execute.OnUIThreadAsync(async () =>
            {
                var categoryItem = new CategoryItem(firstImageFromFeed.imageUrl, firstImageFromFeed.Category);
                CategoryItems.Add(categoryItem);
            });

        }

        private MainPageView _view2;

        protected internal override async Task OnViewAttached(object view, object context)
        {
            _view2 = view as MainPageView;
        }

        private Stream _image;

        private async Task GetWallpaperOfTheDay()
        {
          
            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();

            byte[] imageBytes = null;
            bool shouldGet = false;
            try{
                imageBytes = await BlobCache.LocalMachine.GetAsync(imageMetaData.imageUrl);
            }catch(Exception e)
            {
                shouldGet = true;
             
            }
            if (shouldGet)
            {
                using (var client = new HttpClient())
                {
                    imageBytes = await client.GetByteArrayAsync(imageMetaData.imageUrl);
                    await BlobCache.LocalMachine.Insert(imageMetaData.imageUrl, imageBytes);
                }
            }
            _image = new MemoryStream(imageBytes);


            //BECAUSE WP8 SAID SO
            BitmapImage image = new BitmapImage();
            image.SetSource(_image);
            ImageBrush brush = new ImageBrush
            {
                Opacity= 0.4,
                Stretch = Stretch.Fill,
                ImageSource = image
            };
            Execute.BeginOnUIThread(() =>
            {
                _view2.Panorama.Background = brush;
            });
           
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