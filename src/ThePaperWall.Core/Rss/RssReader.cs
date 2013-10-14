﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core.Rss
{
    public class RssReader : IRssReader
    {
        public async Task<rss> GetFeed(string url)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(rss));
            rss feed = null;
            XmlReader reader = await Task.Run(() => XmlReader.Create(url));
            using (reader)
            {
                feed = (rss)serializer.Deserialize(reader);
            }
            return feed;
        }

        public List<ImageMetaData> GetImageMetaData(rss feed)
        {
            var images = new List<ImageMetaData>();
            foreach (var rssChannelItem in feed.channel.item)
            {
                var html = rssChannelItem.description;
                var imageUrl = GetImageUrl(html);
                images.Add(new ImageMetaData { imageThumbnail = imageUrl });
            }
            return images;
        }

        private string GetImageUrl(string html)
        {
            Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.IgnoreCase);
            string rawString = html;
            return linkParser.Matches(rawString)[1].Value;
        }
    }
}
