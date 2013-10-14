using Caliburn.Micro;
using ReactiveCaliburn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using Windows.UI.Xaml.Media;
using Splat;
using ThePaperWall.WinRT.Data;
using ThePaperWall.WinRT.Common;
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

        protected override async Task OnActivate()
        {
            GetMainHeroImage();

            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }
  
        private void GetMainHeroImage()
        {
            System.Action getMainHeroImage =async () =>
            {              
                var themes = _themeService.GetThemes(WallpaperResource.Feeds);


                var rssForFeed = await _rssReader.GetFeed(themes.WallPaperOfTheDay.FeedUrl);
                var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).First();

                MainImage = (await _downloadManager.DownloadImage(imageMetaData)).ToNative();
            };
            getMainHeroImage.OnUIThreadAsync();
        
                
        }

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private ImageSource _mainImage;
        public ImageSource MainImage
        {
            get { return _mainImage; }
            set { this.RaiseAndSetIfChanged(ref _mainImage, value); }
        }
    }
}
