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

            await GetWallpaperOfTheDay();


           
            BitmapImage image = new BitmapImage();
            image.SetSource(_image);

            
            ImageBrush brush = new ImageBrush
            {

                Stretch = Stretch.Fill,
                ImageSource = image

            };
            Execute.BeginOnUIThread(() => {
                _view2.Panorama.Background = brush;
            });
           
            //GetTop4WallPaperItems();
            //GetCategoryItems();
        }

        private MainPageView _view2;

        protected internal override async Task OnViewAttached(object view, object context)
        {
            _view2 = view as MainPageView;
        }

        private Stream _image;

        private async Task GetWallpaperOfTheDay()
        {
            _themes = _themeService.GetThemes(WallpaperResource.Feeds);
            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();

            //Task<IBitmap> lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);
            //Task<IBitmap> imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);

            //var lowResImage = await lowResImageTask;
            //await Execute.OnUIThreadAsync(async () => WallpaperOfTheDay = (lowResImage).ToNative());
            //_wallpaper = await imageTask;

            //await Execute.OnUIThreadAsync(async () => WallpaperOfTheDay = (_wallpaper).ToNative());
   
            using (var client = new HttpClient())
            {
                var temp = await client.GetByteArrayAsync(imageMetaData.imageUrl);
                _image = new MemoryStream(temp);
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