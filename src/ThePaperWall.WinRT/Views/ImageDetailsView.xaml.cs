using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ReactiveUI;
using ThePaperWall.WinRT.Common;
using ThePaperWall.WinRT.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace ThePaperWall.WinRT.Views
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ImageDetailsView : Page
    {
        public ImageDetailsView()
        {
            this.WhenAnyValue(_ => _.ViewModel.CommandBarIsOpen).Subscribe(_ => CommandBar.IsOpen = _);
        }
        public ImageDetailsViewModel ViewModel { get { return DataContext as ImageDetailsViewModel; } }

        private void CommandBar_Closed(object sender, object e)
        {
            var vm = DataContext as ImageDetailsViewModel;
            vm.CommandBarIsOpen = false;
        }
    }

 
}
