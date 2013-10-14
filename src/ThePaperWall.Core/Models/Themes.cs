using System.Collections.Generic;
using System.Linq;

namespace ThePaperWall.Core.Models
{
    public class Themes
    {
        public Themes(IEnumerable<Theme> all)
        {
            _all = all;
        }
        private IEnumerable<Theme> _all;

        public IEnumerable<Theme> All
        {
            get
            {
                return _all;
            }
            
        }

        public Theme WallPaperOfTheDay
            {
                get
                {
                    return _all.First(t => t.Name == "Wallpaper of the Day");
                }                
            }
    }
}
