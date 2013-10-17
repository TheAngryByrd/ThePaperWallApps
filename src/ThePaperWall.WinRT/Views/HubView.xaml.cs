using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using ThePaperWall.WinRT;
using ThePaperWall.WinRT.Common;
using ThePaperWall.WinRT.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Splat;
using Windows.UI.Core;
using Akavache;
using System.Reactive.Linq;

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
        }
    }
}
