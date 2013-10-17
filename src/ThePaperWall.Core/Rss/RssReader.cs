using Akavache;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ThePaperWall.Core.Models;
using System.Reactive.Linq;
using System.Net.Http;
using System.IO;
using Punchclock;

namespace ThePaperWall.Core.Rss
{
    public class RssReader : IRssReader
    {
        static OperationQueue opQueue = new OperationQueue(10);
        public async Task<rss> GetFeed(string url)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(rss));
            rss feed = null;

            //var rssFeed = await BlobCache.LocalMachine.GetOrFetchObject(url, () => FetchRssFeedQueued(url), DateTimeOffset.Now.AddMinutes(30));
            var rssFeed = await FetchRssFeedQueued(url);
            var reader = XmlReader.Create(new MemoryStream(rssFeed));

            using (reader)
            {
                feed = (rss)serializer.Deserialize(reader);
            }
            return feed;
        }
        public Task<byte[]> FetchRssFeedQueued(string url)
        {
            return opQueue.Enqueue(1, () => FetchRssFeed(url));
        }
        public async Task<byte[]> FetchRssFeed(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetByteArrayAsync(url);
            }
        }

        public List<ImageMetaData> GetImageMetaData(rss feed)
        {
            var images = new List<ImageMetaData>();
            foreach (var rssChannelItem in feed.channel.item)
            {
                var html = rssChannelItem.description;
                var imageUrl = GetImageUrl(html);
                images.Add(new ImageMetaData(imageUrl){ Category = rssChannelItem.title });
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
