using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ThePaperWall.WP8.ViewModels;

namespace ThePaperWall.WP8.Views
{
    public partial class MainPageView : PhoneApplicationPage
    {
        // Constructor
        public MainPageView()
        {
            InitializeComponent();

            (App.Current as App).reminder.Notify();
            
        }
        public MainPageViewModel ViewModel
        {
            get { return (MainPageViewModel)DataContext; }
        }
        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedCategory = e.AddedItems[0] as CategoryItem;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ViewModel.SelectedTop4 = e.AddedItems[0] as CategoryItem;
                var listbox = sender as ListBox;
                listbox.SelectedItem = null;
            }
           
        }
    }
}