using System;
using System.Linq;
using ReactiveUI;
using ThePaperWall.WinRT.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using ThePaperWall.Helpers;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=??????

namespace ThePaperWall.WinRT.Views
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubView : Page
    {
        public HubView()
        {
            this.InitializeComponent();
            Loaded += HubView_Loaded;
        }

        private HubViewModel ViewModel 
        {
            get 
            {
                return (HubViewModel)DataContext;
            }
        }

        void HubView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
           this.ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
           ViewModel.OnActivateCommand.IsExecuting.ToVisibility().Subscribe(isExecuting => ProgressBar.Visibility = isExecuting);
        }

        private void loadingThis(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
           
        }
        

    }
}
