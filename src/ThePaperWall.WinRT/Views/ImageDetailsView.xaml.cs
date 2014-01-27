using System;
using System.Linq;
using ReactiveOperators;
using ReactiveUI;
using ThePaperWall.WinRT.ViewModels;
using Windows.UI.Xaml.Controls;
using ThePaperWall.Helpers;

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
            this.Loaded += ImageDetailsView_Loaded;
        }

        void ImageDetailsView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AreAnyCommandsExecuting().Subscribe(x => ProgressBar.Visibility = x.ToVisiblity());
        }

        private IObservable<bool> AreAnyCommandsExecuting()
        {
            return ViewModel.DownloadPhotoCommand.IsExecuting
                .Or(ViewModel.SetLockscreenCommand.IsExecuting)
                .Or(ViewModel.OnActivatedCommand.IsExecuting);
        }

        public ImageDetailsViewModel ViewModel { get { return DataContext as ImageDetailsViewModel; } }
    }

 
}
