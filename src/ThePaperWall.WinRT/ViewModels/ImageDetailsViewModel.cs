using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Popups;

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

        public ReactiveCommand OnActivatedCommand { get; private set; }
        public ReactiveCommand SetLockscreenCommand { get; private set; }
        public ReactiveCommand DownloadPhotoCommand { get; private set; }

        public ImageDetailsViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager)
        {
            _themeService = themeService;
            _rssReader = rssReader;
            _downloadManager = downloadManager;

            OnActivatedCommand = new ReactiveCommand();
            OnActivatedCommand.RegisterAsyncTask(_ =>   OnActivated());

            SetLockscreenCommand = new ReactiveCommand();
            SetLockscreenCommand.RegisterAsyncTask(_ => SetLockscreen());

            DownloadPhotoCommand = new ReactiveCommand();
            DownloadPhotoCommand.RegisterAsyncTask(_ => DownloadPhoto());
        }
  
        private async Task DownloadPhoto()
        {
            try
            {
                FileSavePicker saver = new FileSavePicker();
                saver.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                saver.SuggestedFileName = Title;
                saver.FileTypeChoices.Add(".jpg", new List<string> { ".jpg" });
                var storageFile = await saver.PickSaveFileAsync();

                if (storageFile != null)
                {
                    FileIO.WriteBufferAsync(storageFile, (await GetImageStream()).ToArray().AsBuffer());
                    MessageDialog md = new MessageDialog("Image has been saved your pictures.");
                    await md.ShowAsync();
                }
            }
            catch (Exception e)
            {
            }
        }
  
        private async Task SetLockscreen()
        {
            try
            {
                var memoryStream = await GetImageStream();            
                await LockScreen.SetImageStreamAsync(WindowsRuntimeStreamExtensions.AsRandomAccessStream(memoryStream));
                MessageDialog md = new MessageDialog("Image has been set as your lockscreen.");
                await md.ShowAsync();
            }
            catch (Exception e)
            {
            }
        }
  
        private async Task<MemoryStream> GetImageStream()
        {
            var memoryStream = new MemoryStream();            
            await _image.Save(CompressedBitmapFormat.Jpeg, 1, memoryStream);
            return memoryStream;
        }

        private IBitmap _image;

        protected override async Task OnActivate()
        {
            OnActivatedCommand.Execute(null);
        }
  
        private async Task OnActivated()
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
