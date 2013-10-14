using ThePaperWall.Core.Models;

namespace ThePaperWall.Core.Feeds
{
    public interface IThemeService
    {
        Themes GetThemes(string feed);
    }
}