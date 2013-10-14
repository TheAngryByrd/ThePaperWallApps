using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ReactiveCaliburn;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using ThePaperWall.WinRT.Common;
using ThePaperWall.WinRT.Data;
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
            GetMainHeroImage();
            GetTop4WallPaperItems();

            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }
  
        private void GetTop4WallPaperItems()
        {
            System.Action getMainHeroImage = async () =>
            {
                var rssForFeed = await _rssReader.GetFeed(_themes.Top4.FeedUrl);
                var imageMetaData = _rssReader.GetImageMetaData(rssForFeed);

               foreach(var imd in imageMetaData)
               {
                    Top4Items.Add(new Top4WallPaperItem(_downloadManager,imd));
               }
            };
            getMainHeroImage.OnUIThreadAsync(); 

        }
  
        private void GetMainHeroImage()
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

        public ObservableCollection<Top4WallPaperItem> _top4Items = new ObservableCollection<Top4WallPaperItem>();

        public ObservableCollection<Top4WallPaperItem> Top4Items
        {
            get { return _top4Items; }
        }

        private ImageSource _mainImage;
        public ImageSource MainImage
        {
            get { return _mainImage; }
            set { this.RaiseAndSetIfChanged(ref _mainImage, value); }
        }
    }

    public class Top4WallPaperItem : ReactiveObject
    {

        public Top4WallPaperItem(IAsyncDownloadManager downloaderManager, ImageMetaData imageMetaData)
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
