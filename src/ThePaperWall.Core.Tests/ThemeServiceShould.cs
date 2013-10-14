using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;

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

        [TestMethod]
        public async Task GetImageInformation()
        {
              var themeService = new ThemeService(WallpaperResource.Feeds);
            var themes = themeService.GetThemes();

            var fixture = new RssReader();

            var rssForFeed = await fixture.GetFeed(themes.WallPaperOfTheDay.FeedUrl);
            var images = fixture.GetImageUrls(rssForFeed);
            Assert.IsNotNull(images.First().imageUrl);

        }
    }
}
