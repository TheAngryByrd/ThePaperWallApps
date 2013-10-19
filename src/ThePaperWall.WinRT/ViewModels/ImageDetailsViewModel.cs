using System;
using System.Linq;
using Caliburn.Micro;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace ThePaperWall.WinRT.ViewModels
{
    public class ImageDetailsViewModel : BaseReactiveScreen
    {
        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;
        private readonly INavigationService _navigationService;

        

        public string Category { get; set; }

        public string Title { get; set; }

        public ImageDetailsViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager,
            INavigationService navigationService)
        {
            _themeService = themeService;
            _rssReader = rssReader;
            _downloadManager = downloadManager;
            _navigationService = navigationService;
        }

        protected override async System.Threading.Tasks.Task OnActivate()
        {
            var theme = _themeService.GetThemes().Categories.First(c => c.Name == Category);
            var feed = await _rssReader.GetFeed(theme.FeedUrl);
            var imageMetaData = _rssReader.GetImageMetaData(feed).First(img => img.Category == Title);

            

            Task<IBitmap> lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);
            Task<IBitmap> imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);

            var lowResImage = await lowResImageTask;
            await Execute.OnUIThreadAsync(() => ImageSource = (lowResImage).ToNative());
            var image = await imageTask;
            await Execute.OnUIThreadAsync(() => ImageSource = (image).ToNative());
            ProgressBarIsVisible = false;

        }
        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref imageSource,value);
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
    }
}
