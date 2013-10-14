using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThePaperWall.Core.Models;

namespace ThePaperWall.Core
{
    public interface IAsyncDownloadManager
    {
        Task<IBitmap> DownloadImage(ImageMetaData image, IProgress<ProgressEvent> progress);    
    }

    public class AsyncDownloadManager : IAsyncDownloadManager
    {
        public async Task<IBitmap> DownloadImage(ImageMetaData image, IProgress<ProgressEvent> progress)
        {            
            using (HttpClient wc = new HttpClient())
            {
                var stream = await wc.GetStreamAsyncWithProgress(image.imageUrl, progress);
                return await BitmapLoader.Current.Load(stream, null, null);
            }           
        }
    }

    public class ProgressEvent
    {
        public long TotalBytes { get; set; }
        public long BytesRead { get; set; }
        public double BytesPerSecond { get; set; }
        public decimal Percent()
        {
            if (TotalBytes == 0L)
                return 0M;

            return (decimal)BytesRead / (decimal)TotalBytes;
        }
    }
    public static class HttpClientExtensions
    {
        public static async Task<Stream> GetStreamAsyncWithProgress(this HttpClient client,
            string requestUri,
            IProgress<ProgressEvent> onProgress = null,
            int bufferLength = 524288 /*512KB*/)
        {
            List<byte> result = new List<byte>();
            byte[] buffer;
            var bytesRead = 0L;
            var bytesPerSecond = 0L;
            var watch = new Stopwatch();

            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
            {
                long totalBytes = 0L;
                try
                {
                    // web streams aren't seekable, so get the length from the headers
                    IEnumerable<string> contentLengthValues;
                    if (response.Content.Headers.TryGetValues("Content-Length", out contentLengthValues))
                    {
                        if (!long.TryParse(contentLengthValues.First(), out totalBytes))
                            totalBytes = 0L;
                    }
                }
                catch (Exception)
                {
                    totalBytes = 0L;
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    watch.Start();
                    while (stream.CanRead)
                    {
                        buffer = new byte[bufferLength];
                        // web streams aren't seekable, so the offset is always 0
                        var read = await stream.ReadAsync(buffer, 0, bufferLength);
                        if (read > 0)
                        {
                            if (read == bufferLength)
                                result.AddRange(buffer);
                            else
                                result.AddRange(buffer.Take(read));

                            bytesRead += read;
                            bytesPerSecond += read;

                            if (onProgress != null)
                            {
                                var seconds = watch.Elapsed.TotalSeconds;
                                if (seconds >= 1D)
                                {
                                    var eventArgs = new ProgressEvent()
                                    {
                                        BytesRead = bytesRead,
                                        TotalBytes = totalBytes
                                    };
                                    eventArgs.BytesPerSecond = bytesPerSecond / seconds;
                                    onProgress.Report(eventArgs);

                                    bytesPerSecond = 0L;
                                    watch.Reset();
                                    watch.Start();
                                }
                            }
                        }
                        else
                            break;
                    }
                }
            }

            var responseBytes = result.ToArray();
            result = null;
            buffer = null;
            watch.Stop();
            watch.Reset();
            watch = null;
            return new MemoryStream(responseBytes);          
        }
    }
}
