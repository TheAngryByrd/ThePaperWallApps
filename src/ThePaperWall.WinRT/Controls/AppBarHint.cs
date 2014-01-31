using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ThePaperWall.WinRT.Controls
{
    public class AppBarHint : Button
    {
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (Window.Current.Content is MyToolkit.Paging.Frame)
            {
                var frame = (MyToolkit.Paging.Frame)Window.Current.Content;               
                var page = frame.CurrentPage.Page.InternalPage;
                if (page.BottomAppBar != null)
                {
                    page.BottomAppBar.IsOpen = true;
                }                
            }
        }
    }
}
