using System;
using System.Linq;
using ReactiveUI;
using ThePaperWall.WinRT.ViewModels;
using Windows.UI.Xaml.Controls;

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
            this.Loaded += loadingThis;
        }

        private void loadingThis(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.WhenAnyValue(_ => _.ViewModel.CommandBarIsOpen).Subscribe(_ => CommandBar.IsOpen = _);
        }
        
        public HubViewModel ViewModel { get { return DataContext as HubViewModel; } }

        private void CommandBar_Closed(object sender, object e)
        {
            var vm = DataContext as HubViewModel;
            vm.CommandBarIsOpen = false;
           
        }
    }
}
