using System.Resources;
using System.Reflection;

namespace ThePaperWall.Core
{
    public class WallpaperResource
    {
        static ResourceManager ResourceManager = new ResourceManager("ThePaperWall.Core.WallpaperResource", typeof(WallpaperResource).GetTypeInfo().Assembly);

        public static string Feeds
        {
            get
            {
                return ResourceManager.GetString("Feeds");
            }
        }
    }

   
}
