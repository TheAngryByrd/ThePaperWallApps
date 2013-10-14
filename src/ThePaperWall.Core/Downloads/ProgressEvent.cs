namespace ThePaperWall.Core.Downloads
{
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
}