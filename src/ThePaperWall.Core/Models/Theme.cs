namespace ThePaperWall.Core.Models
{
    public class Theme
    {
        public string Name { get; set; }
        public string FeedUrl { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
