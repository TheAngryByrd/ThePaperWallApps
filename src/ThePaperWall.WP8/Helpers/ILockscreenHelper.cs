using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using ThePaperWall.Core.Models;
using Windows.Phone.System.UserProfile;

namespace ThePaperWall.WP8.Helpers
{
    public interface ILockscreenHelper
    {
        Task SetLockscreen(string url);
    }
}