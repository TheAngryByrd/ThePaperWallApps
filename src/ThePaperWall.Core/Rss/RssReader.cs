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
           // BlobCache.LocalMachine.Dispose();
            bool shouldGet = false;
            byte[] rssFeed = null;
            try
            {
                rssFeed = await BlobCache.LocalMachine.GetAsync(url);
            }
            catch(Exception e)
            {
                shouldGet = true;
            }
            if (shouldGet)
            {
                rssFeed = await FetchRssFeedQueued(url);
                await BlobCache.LocalMachine.Insert(url, rssFeed);
            }
         
            var reader = XmlReader.Create(new MemoryStream(rssFeed));

            using (reader)
            {
                try
                {
                    feed = (rss)serializer.Deserialize(reader);
                    
                }
                catch (Exception e)
                {
                }
            }
            return feed;
        }
        private Task<byte[]> FetchRssFeedQueued(string url)
        {
            return opQueue.Enqueue(1, () => FetchRssFeed(url));
        }
        private async Task<byte[]> FetchRssFeed(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetByteArrayAsync(url);
                }
                catch (Exception e)
                {                   
                    throw e;
                }
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
