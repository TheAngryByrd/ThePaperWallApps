using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Models;

namespace ThePaperWall.WP8.Services
{
    public interface INotificationService
    {
        Task CreateLiveTileFromImageMetadata(List<ImageMetaData> images);
    }
}