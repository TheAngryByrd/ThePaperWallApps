using System.Collections.Generic;
using System.Threading.Tasks;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core.Rss
{
    public interface IRssReader
    {
        ImageMetaData GetFirstImageMetaData(rss feed);

        Task<rss> GetFeed(string url);

        List<ImageMetaData> GetImageMetaData(rss feed);
    }
}