﻿using System.Collections.Generic;
using System.Linq;

namespace ThePaperWall.Core.Models
{
    public class Themes
    {
        public Themes(IEnumerable<Theme> all)
        {
            _all = all.ToList();
        }
        private IList<Theme> _all;

        public IEnumerable<Theme> All { get { return _all; } }

        public Theme WallPaperOfTheDay { get { return _all[0]; } }
        public Theme Top4 { get { return _all[1]; } }
        public Theme Recent50 { get { return _all[2]; } }   

        public IEnumerable<Theme> Categories
        {
            get { return _all.Except(new[] { WallPaperOfTheDay, Top4, Recent50 }.OrderBy(t => t.Name)); }
        }
    }
}
