using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThePaperWall.Core.Tests
{
    [TestClass]
    public class ThemeServiceShould
    {
        [TestMethod]
        public void GetThemesFromStringResource()
        {
            var fixture = new ThemeService(WallpaperResource.Feeds);
            var themes = fixture.GetThemes();

            Assert.AreEqual("Wallpaper of the Day", themes.WallPaperOfTheDay.Name);
            Assert.AreEqual("Top 4 Wallpapers Today", themes.All.ToList()[1].Name);
            Assert.AreEqual(37, themes.All.Count());
        }
    }
}
