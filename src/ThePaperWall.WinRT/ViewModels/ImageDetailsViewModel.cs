using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ReactiveUI;
using Splat;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Models;
using ThePaperWall.Core.Rss;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.IO;

namespace ThePaperWall.WinRT.ViewModels
{
    public class ImageDetailsViewModel : BaseReactiveScreen
    {
        private readonly IThemeService _themeService;
        private readonly IRssReader _rssReader;
        private readonly IAsyncDownloadManager _downloadManager;

       

        public string Category { get; set; }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _title, value);
            }
        }
        /// <summary>
        /// This should be the URL because there's no other way to Id
        /// this in the feed
        /// </summary>
        public string Id { get; set; }

        public ReactiveCommand SetLockscreenCommand { get; private set; }

        public ImageDetailsViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager)
        {
            _themeService = themeService;
            _rssReader = rssReader;
            _downloadManager = downloadManager;

            SetLockscreenCommand = new ReactiveCommand();
            SetLockscreenCommand.Subscribe(_ => SetLockscreen());
        }
  
        private async void SetLockscreen()
        {
            try
            {
                var memoryStream = new MemoryStream();            
                await _image.Save(CompressedBitmapFormat.Jpeg, 1, memoryStream);            
                await LockScreen.SetImageStreamAsync(WindowsRuntimeStreamExtensions.AsRandomAccessStream(memoryStream));
            }
            catch (Exception e)
            {
            }
        }

        private IBitmap _image;

        protected override async Task OnActivate()
        {
            var theme = _themeService.GetThemes().All.First(c => c.Name == Category);
            var feed = await _rssReader.GetFeed(theme.FeedUrl);
            List<ImageMetaData> imageDataFromFeed = _rssReader.GetImageMetaData(feed);
            ImageMetaData imageMetaData = null;

            if (!string.IsNullOrEmpty(Id))
                imageMetaData = imageDataFromFeed.First(img => img.imageUrl == Id);            
            else            
                imageMetaData = imageDataFromFeed.First();                
            

            Title = imageMetaData.Category;

            Task<IBitmap> lowResImageTask = _downloadManager.DownloadImage(imageMetaData.imageThumbnail, priority: 10);
            Task<IBitmap> imageTask = _downloadManager.DownloadImage(imageMetaData.imageUrl, priority: 10);
            
            var lowResImage = await lowResImageTask;
            await Execute.OnUIThreadAsync(() => ImageSource = (lowResImage).ToNative());
            _image = await imageTask;
            await Execute.OnUIThreadAsync(() => ImageSource = (_image).ToNative());
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

        private bool _commandBarIsOpen;
        public bool CommandBarIsOpen
        {
            get
            {
                return this._commandBarIsOpen;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _commandBarIsOpen, value);
            }
        }
    }
}
