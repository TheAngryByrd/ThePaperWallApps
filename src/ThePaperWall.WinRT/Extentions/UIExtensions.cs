using System;
using System.Reactive.Linq;
#if NETFX_CORE
using Windows.UI.Xaml;
#elif WP8
using System.Windows;
#endif

namespace ThePaperWall.Helpers
{
    public static class UIExtensions 
    {
        public static Visibility ToVisibility(this bool This)
        {
            return This ? Visibility.Visible : Visibility.Collapsed;
        }

        public static IObservable<Visibility> ToVisibility(this IObservable<bool> This)
        {
            return This.Select(x => x.ToVisibility());
        }
    }
}