
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
    public partial class FullScreenImage : PhoneApplicationPage
    {
        public FullScreenImage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (this.NavigationContext.QueryString.ContainsKey("item"))
            {
                string queryString = this.NavigationContext.QueryString["item"];
                this.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in this.slideView.ItemsSource)
                    {
                        if (item.ToString().Equals(queryString))
                        {
                            this.slideView.SelectedItem = item;
                            return;
                        }
                    }
                });
            }
        }
    }
}
