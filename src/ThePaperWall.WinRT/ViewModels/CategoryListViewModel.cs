﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ReactiveUI;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using Splat;
using ThePaperWall.Core.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ThePaperWall.ViewModels;

namespace ThePaperWall.WinRT.ViewModels
{
    public class CategoryListViewModel : BaseReactiveScreen
    {
        private readonly IThemeService themeService;
        private readonly IRssReader rssReader;
        private readonly IAsyncDownloadManager downloadManager;
        private readonly INavigationService navigationService;

        public ReactiveCommand OnActivateCommand { get; private set; }

        public CategoryListViewModel(IThemeService themeService,
            IRssReader rssReader,
            IAsyncDownloadManager downloadManager,
            INavigationService navigationService)
        {
            this.themeService = themeService;
            this.rssReader = rssReader;
            this.downloadManager = downloadManager;
            this.navigationService = navigationService;

            OnActivateCommand = new ReactiveCommand();
            OnActivateCommand.RegisterAsyncTask(_ => GetImages());
            CategoryItemCommand = new ReactiveCommand();
            CategoryItemCommand.Subscribe(item => 
                {
                    var categoryItem = item as CategoryItem;
                    navigationService
                        .UriFor<ImageDetailsViewModel>()
                        .WithParam(x => x.Category, Category)
                        .WithParam(x => x.Id, categoryItem.Id)
                        .Navigate();
                });
        }

        public string Category { get; set; }
        public ObservableCollection<CategoryItem> _categoryItems = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> CategoryItems
        {
            get { return _categoryItems; }
        }
        protected override async Task OnActivate()
        {
            OnActivateCommand.Execute(null);
        }
  
        private async Task GetImages()
        {
            var theme = themeService.GetThemes().Categories.First(c => c.Name == Category);
            var feed = await rssReader.GetFeed(theme.FeedUrl);
            var images = rssReader.GetImageMetaData(feed);
            await Task.WhenAll(images.Select(x => CreateCategoryItem(x)));           
        }

        private async Task CreateCategoryItem(ImageMetaData imageMetaData)
        {
            Func<Task<IBitmap>> lazyImageFactory = () => downloadManager.DownloadImage(imageMetaData.imageThumbnail);
            var category = new CategoryItem(imageMetaData.imageUrl, imageMetaData.Category, lazyImageFactory);
            CategoryItems.Add(category);
            await category.LoadImage();
        }

        public ReactiveCommand CategoryItemCommand { get; private set; }   
    }
}
