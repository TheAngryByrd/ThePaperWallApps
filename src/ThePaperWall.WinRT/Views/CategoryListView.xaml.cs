using ThePaperWall.WinRT.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using ThePaperWall.Helpers;
using ThePaperWall.WinRT.ViewModels;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace ThePaperWall.WinRT.Views
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class CategoryListView : MyToolkit.Paging.Page
    {
        public CategoryListView()
        {
            Loaded += CategoryListView_Loaded;
        }

        void CategoryListView_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.OnActivateCommand.IsExecuting.Subscribe(x => ProgressBar.Visibility = x.ToVisibility());
        }

        private CategoryListViewModel ViewModel 
        {
            get
            {
                return DataContext as CategoryListViewModel;
            }
        }
    }
}
