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
                var imageMetaData = _rssReader.GetImageMetaData(rssForFeed).Take(1);

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
        private AsyncLazy<IBitmap> _lazyImage;

        public Top4WallPaperItem(IAsyncDownloadManager downloaderManager, ImageMetaData imageMetaData)
        {
            Category = imageMetaData.Category;
            System.Action lazyImage = async () => 
                {
                    ImagePath = (await downloaderManager.DownloadImage(imageMetaData.imageThumbnail)).ToNative();
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

    /// <summary>
    /// Provides support for lazy initialization in asyncronous manner.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    public sealed class AsyncLazy<T>
    {
        #region Fields

        private object _syncObject = new object();
        private Task<T> _initializeTask;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy"/> class.
        /// When lazy initialization occurs, the specified initialization function is used.
        /// </summary>
        /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when
        /// it is needed.</param>
        public AsyncLazy(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _initializeTask = new Task<T>(valueFactory);
        }

        public AsyncLazy(Task<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _initializeTask = valueFactory;
        }

        #endregion

        /// <summary>
        ///  Gets the lazily initialized value of the current instance.
        ///  If value have not been initialized, block calling thread until value get initialized.
        ///  If during initialization exception have been thrown, it will wrapped into <see cref="AggregateException"/>
        ///  and rethrowned on accessing this property.
        /// </summary>
        public T Value
        {
            get
            {
                if (_initializeTask.Status == TaskStatus.Created)
                {
                    lock (_syncObject)
                    {
                        if (_initializeTask.Status == TaskStatus.Created)
                        {
                            _initializeTask.RunSynchronously();
                        }
                    }
                }

                return _initializeTask.Result;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a value has been created for this instance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a value has been created; otherwise, <c>false</c>.
        /// </value>
        public bool IsValueCreated
        {
            get
            {
                return _initializeTask.IsCompleted;
            }
        }

        /// <summary>
        /// Initializes value on background thread.
        /// Calling thread will never be blocked.
        /// </summary>
        public void InitializeAsync()
        {
            if (_initializeTask.Status == TaskStatus.Created)
            {
                lock (_syncObject)
                {
                    if (_initializeTask.Status == TaskStatus.Created)
                    {
                        _initializeTask.Start();
                    }
                }
            }
        }
    }
  
}
