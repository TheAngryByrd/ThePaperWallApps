using System.Collections.Generic;
using System.Threading.Tasks;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core.Rss
{
    public interface IRssReader
    {
        Task<rss> GetFeed(string url);

        List<ImageMetaData> GetImageMetaData(rss feed);
    }
}