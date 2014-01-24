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

            if (Window.Current.Content is Frame)
            {
                var frame = (Frame)Window.Current.Content;
                if (frame.Content is Page)
                {
                    var page = (Page)frame.Content;
                    if (page.BottomAppBar != null)
                    {
                        page.BottomAppBar.IsOpen = true;
                    }
                }
            }
        }
    }
}
