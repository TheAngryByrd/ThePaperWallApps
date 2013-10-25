
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

namespace ThePaperWall.WP8.Views
{
    public partial class CategoryListView : PhoneApplicationPage
    {
        public CategoryListView()
        {
            InitializeComponent();
        }

        private void slideView_Tap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
           // this.NavigationService.Navigate(new Uri("/Views/FullScreenImage.xaml?item=" + this.slideView.SelectedItem.ToString(), UriKind.RelativeOrAbsolute));
        }
    }
}
