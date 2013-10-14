using System;
using System.Collections.Generic;
using System.Linq;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core
{
    public class ThemeService : IThemeService
    {
        private string feed;

        public ThemeService(string feed)
        {
            this.feed = feed;
        }

        public Themes GetThemes()
        {
            var themes =feed.Split(',').AsEnumerable().Buffer(2).Select(x => new Theme { Name = x[0].Trim(), FeedUrl = x[1].Trim() });
           
            return new Themes(themes);
        }
    }
}
