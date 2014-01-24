#if NETFX_CORE
using Windows.UI.Xaml;
#elif WP8
using System.Windows;
#endif

namespace ThePaperWall.Helpers
{
    public static class UIExtensions 
    {
        public static Visibility ToVisiblity(this bool This)
        {
            return This ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}