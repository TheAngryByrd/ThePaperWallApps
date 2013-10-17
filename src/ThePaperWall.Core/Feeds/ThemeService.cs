using System.Collections.Generic;
using System.Linq;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core.Feeds
{
    public class ThemeService : IThemeService
    {   

        public Themes GetThemes(string feed)
        {
            var themes = feed.Split(',');
            var foo = themes.AsEnumerable();
            var foo1 = foo.Split2(1).ToList();
            var foo2 = foo1.Select(x => new Theme 
                    {
                        Name = x.First().Trim(),
                        FeedUrl = x.Last().Trim()
                    });

            return new Themes(foo2);
        }
    }

    public static class EnumberEx
    {
        public static IEnumerable<IEnumerable<T>> Split2<T>(this IEnumerable<T> source, int chunkSize)
        {
            var chunk = new List<T>(chunkSize);
            foreach (var x in source)
            {
                chunk.Add(x);
                if (chunk.Count <= chunkSize)
                {
                    continue;
                }
                yield return chunk;
                chunk = new List<T>(chunkSize);
            }
            if (chunk.Any())
            {
                yield return chunk;
            }
        }
    }
}
