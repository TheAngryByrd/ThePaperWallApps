using System;
using System.Linq;
using Caliburn.Micro;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using Windows.UI.Xaml.Media;

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


            var lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);
            var imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);

            await Execute.OnUIThreadAsync(async () => ImageSource = (await lowResImageTask).ToNative());
             await Execute.OnUIThreadAsync(async () => ImageSource = (await imageTask).ToNative());

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
    }
}
