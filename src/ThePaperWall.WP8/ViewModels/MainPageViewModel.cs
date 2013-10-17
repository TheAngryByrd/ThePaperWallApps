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
            GetWallpaperOfTheDay();
            //GetTop4WallPaperItems();
            //GetCategoryItems();
        }

        private async void GetWallpaperOfTheDay()
        {
            var rssForFeed = await _rssReader.GetFeed(_themes.WallPaperOfTheDay.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();
            var image= await _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);
            //var lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);


            //await Execute.OnUIThreadAsync(async () => 
            //    {
            //        WallpaperOfTheDay = (await lowResImageTask).ToNative();                    
            //    });
            Execute.BeginOnUIThread(async () =>
            {                
                WallpaperOfTheDay = image.ToNative();
            });
        }

        private ImageSource _wallpaperOfTheDay;
        public ImageSource WallpaperOfTheDay
        {
            get { return _wallpaperOfTheDay; }
            set { this.RaiseAndSetIfChanged(ref _wallpaperOfTheDay, value); }
        }
    }
}