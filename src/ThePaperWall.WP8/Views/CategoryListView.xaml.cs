
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ThePaperWall.WP8.ViewModels;

namespace ThePaperWall.WP8.Views
{
    public partial class CategoryListView : PhoneApplicationPage
    {
        public CategoryListView()
        {
            InitializeComponent();
        }
        public CategoryListViewModel ViewModel
        {
            get
            { return DataContext as CategoryListViewModel; }
        }

        private void slideView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModel.SetLockscreen(this.slideView.SelectedItem as CategoryItem);
           // this.NavigationService.Navigate(new Uri("/Views/FullScreenImage.xaml?item=" + this.slideView.SelectedItem.ToString(), UriKind.RelativeOrAbsolute));
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
