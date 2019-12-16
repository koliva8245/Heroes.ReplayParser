﻿using System;
using System.IO;

namespace Heroes.MpqToolV2
{
    public static class MpqFile
    {
        public static MpqArchive Open(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(fileName));
            }

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, false);

            try
            {
                return new MpqArchive(fileStream);
            }
            catch
            {
                fileStream.Dispose();
                throw;
            }
        }

        public static MpqArchive Open(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new MpqArchive(stream);
        }
    }
}
