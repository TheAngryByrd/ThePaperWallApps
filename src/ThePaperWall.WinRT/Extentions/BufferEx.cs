﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace ThePaperWall.WinRT.Extentions
{
    public static class BufferEx
    {
        public static byte[] AsBytes(this IBuffer buffer)
        {
            var retBytes = new byte[0];

            CryptographicBuffer.CopyToByteArray(buffer, out retBytes);

            return retBytes;
        }

        public static IBuffer AsBuffer(this byte[] bytes)
        {
            return CryptographicBuffer.CreateFromByteArray(bytes);
        }

        public static Stream AsStream(this IDataReader reader)
        {

            MemoryStream stream = new MemoryStream();
            var bytes = reader.DetachBuffer().AsBytes();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }
    }
}
