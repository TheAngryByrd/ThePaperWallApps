using System.Linq;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core.Feeds
{
    public class ThemeService : IThemeService
    {   

        public Themes GetThemes(string feed)
        {
            var themes =feed.Split(',').AsEnumerable().Buffer(2).Select(x => new Theme { Name = x[0].Trim(), FeedUrl = x[1].Trim() });
           
            return new Themes(themes);
        }
    }
}
