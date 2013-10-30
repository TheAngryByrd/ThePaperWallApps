
using System;
using System.Collections.Generic;
using System.Linq;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SlideView;
using System.Threading.Tasks;

namespace ThePaperWall.WP8.Views
{
    public partial class CategoryListView : PhoneApplicationPage
    {
        public CategoryListView()
        {
            InitializeComponent();
            this.listBox.SetValue(InteractionEffectManager.IsInteractionEnabledProperty, true);
            this.slideView.SetValue(InteractionEffectManager.IsInteractionEnabledProperty, true);
            InteractionEffectManager.AllowedTypes.Add(typeof(RadDataBoundListBoxItem));
            InteractionEffectManager.AllowedTypes.Add(typeof(SlideViewItem));
            this.listBox.RealizedItemsBufferScale = 1.5;

            slideView.SlideAnimationCompleted += slideView_SlideAnimationCompleted;
            
        }

        async void slideView_SlideAnimationCompleted(object sender, EventArgs e)
        {
            if(slideView.NextItem == slideView.ItemsSource.ElementAt(0))
            {
                await RequestMoreData();      
            }
        }
        public CategoryListViewModel ViewModel
        {
            get
            { return DataContext as CategoryListViewModel; }
        }

        private void slideView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModel.SetLockscreen(this.slideView.SelectedItem as CategoryItem);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.slideView.SelectedItem = e.AddedItems[0];
           
        }

        private async void listBox_DataRequested(object sender, EventArgs e)
        {
            await RequestMoreData();      
        }
  
        private async Task RequestMoreData()
        {
            await ViewModel.AddMorePictures(3);
        }

    }
}
