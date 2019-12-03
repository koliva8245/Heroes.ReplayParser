using System;
using System.Buffers.Binary;
using System.IO;

namespace Heroes.MpqToolV2
{
    public class MpqArchive : IDisposable
    {
        private static readonly uint[] _stormBuffer;
        private readonly MpqHeader _mpqHeader;
        private readonly BinaryReader _binaryReader;

        private readonly MpqHash[] _mpqHashes;
        private readonly MpqArchiveEntry[] _mpqArchiveEntries;

        private bool _isDisposed = false;

        static MpqArchive()
        {
            _stormBuffer = BuildStormBuffer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpqArchive"/> class.
        /// </summary>
        /// <param name="stream">The stream containing the archive to be read.</param>
        /// <exception cref="ArgumentNullException" />
        internal MpqArchive(Stream stream)
        {
            ArchiveStream = stream ?? throw new ArgumentNullException(nameof(stream));
            _binaryReader = new BinaryReader(ArchiveStream);
            _mpqHeader = new MpqHeader(_binaryReader);

            if (_mpqHeader.HashTableOffsetHigh != 0 || _mpqHeader.ExtendedBlockTableOffset != 0 || _mpqHeader.BlockTableOffsetHigh != 0)
                throw new MpqToolException("MPQ format version 1 features are not supported");

            BlockSize = 0x200 << _mpqHeader.BlockSize;

            // LoadHashTable
            ArchiveStream.Seek(_mpqHeader.HashTablePos, SeekOrigin.Begin);
            byte[] hashData = _binaryReader.ReadBytes((int)(_mpqHeader.HashTableSize * MpqHash.Size));
            DecryptTable(hashData, "(hash table)");

            _mpqHashes = new MpqHash[_mpqHeader.HashTableSize];

            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(hashData)))
            {
                for (int i = 0; i < _mpqHeader.HashTableSize; i++)
                    _mpqHashes[i] = new MpqHash(binaryReader);
            }

            // LoadEntryTable;
            ArchiveStream.Seek(_mpqHeader.BlockTablePos, SeekOrigin.Begin);
            byte[] entrydata = _binaryReader.ReadBytes((int)(_mpqHeader.BlockTableSize * MpqArchiveEntry.Size));
            DecryptTable(entrydata, "(block table)");

            _mpqArchiveEntries = new MpqArchiveEntry[_mpqHeader.BlockTableSize];

            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(entrydata)))
            {
                for (int i = 0; i < _mpqHeader.BlockTableSize; i++)
                    _mpqArchiveEntries[i] = new MpqArchiveEntry(binaryReader, (uint)_mpqHeader.HeaderOffset);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MpqArchive"/> class.
        /// </summary>
        ~MpqArchive()
        {
            Dispose(false);
        }

        public int BlockSize { get; private set; }

        public Stream ArchiveStream { get; }

        public MpqMemory HeaderData => _mpqHeader.HeaderData;

        public MpqMemory OpenFile(string filename)
        {
            if (!TryGetHashEntry(filename, out MpqHash hash))
                throw new FileNotFoundException("File not found: " + filename);

            MpqArchiveEntry entry = _mpqArchiveEntries[hash.BlockIndex];
            if (entry.FileName == null)
                entry.FileName = filename;

            return new MpqMemory(this, entry);
        }

        public MpqMemory OpenFile(MpqArchiveEntry mpqArchiveEntry)
        {
            return new MpqMemory(this, mpqArchiveEntry);
        }

        public bool FileExists(string filename)
        {
            return TryGetHashEntry(filename, out MpqHash hash);
        }

        public bool AddListfileFileNames()
        {
            if (!AddFileName("(listfile)")) return false;

            MpqMemory mpqMemory = OpenFile("(listfile)");

            AddFilenames(mpqMemory);

            //using (Stream s = OpenFile("(listfile)"))
            //    AddFilenames(s);

            return true;
        }

        public void AddFilenames(MpqMemory mpqMemory)
        {
            while (!mpqMemory.IsEndOfBuffer)
                AddFileName(mpqMemory.ReadLine());
            //using (StreamReader sr = new StreamReader(stream))
            //{
            //    while (!sr.EndOfStream)
            //        AddFileName(sr.ReadLine());
            //}
        }

        public bool AddFileName(string fileName)
        {
            if (!TryGetHashEntry(fileName, out MpqHash hash)) return false;

            _mpqArchiveEntries[hash.BlockIndex].FileName = fileName;
            return true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal static uint HashString(ReadOnlySpan<char> input, int offset)
        {
            Span<char> upperInput = stackalloc char[input.Length];

            uint seed1 = 0x7fed7fed;
            uint seed2 = 0xeeeeeeee;

            input.ToUpperInvariant(upperInput);

            foreach (int val in upperInput)
            {
                seed1 = _stormBuffer[offset + val] ^ (seed1 + seed2);
                seed2 = (uint)val + seed1 + seed2 + (seed2 << 5) + 3;
            }

            return seed1;
        }

        internal static void DecryptBlock(Span<byte> data, uint seed1)
        {
            uint seed2 = 0xeeeeeeee;

            // NB: If the block is not an even multiple of 4,
            // the remainder is not encrypted
            for (int i = 0; i < data.Length - 3; i += 4)
            {
                seed2 += _stormBuffer[0x400 + (seed1 & 0xff)];

                uint result = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(i, 4));

                result ^= seed1 + seed2;

                seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
                seed2 = result + seed2 + (seed2 << 5) + 3;

                data[i + 0] = (byte)(result & 0xff);
                data[i + 1] = (byte)((result >> 8) & 0xff);
                data[i + 2] = (byte)((result >> 16) & 0xff);
                data[i + 3] = (byte)((result >> 24) & 0xff);
            }
        }

        internal static void DecryptBlock(uint[] data, uint seed1)
        {
            uint seed2 = 0xeeeeeeee;

            for (int i = 0; i < data.Length; i++)
            {
                seed2 += _stormBuffer[0x400 + (seed1 & 0xff)];
                uint result = data[i];
                result ^= seed1 + seed2;

                seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
                seed2 = result + seed2 + (seed2 << 5) + 3;
                data[i] = result;
            }
        }

        // This function calculates the encryption key based on
        // some assumptions we can make about the headers for encrypted files
        internal static uint DetectFileSeed(uint value0, uint value1, uint decrypted)
        {
            uint temp = (value0 ^ decrypted) - 0xeeeeeeee;

            for (int i = 0; i < 0x100; i++)
            {
                uint seed1 = temp - _stormBuffer[0x400 + i];
                uint seed2 = 0xeeeeeeee + _stormBuffer[0x400 + (seed1 & 0xff)];
                uint result = value0 ^ (seed1 + seed2);

                if (result != decrypted)
                    continue;

                uint saveseed1 = seed1;

                // Test this result against the 2nd value
                seed1 = ((~seed1 << 21) + 0x11111111) | (seed1 >> 11);
                seed2 = result + seed2 + (seed2 << 5) + 3;

                seed2 += _stormBuffer[0x400 + (seed1 & 0xff)];
                result = value1 ^ (seed1 + seed2);

                if ((result & 0xfffc0000) == 0)
                    return saveseed1;
            }

            return 0;
        }

        private bool TryGetHashEntry(ReadOnlySpan<char> filename, out MpqHash hash)
        {
            uint index = HashString(filename, 0);
            index &= _mpqHeader.HashTableSize - 1;
            uint name1 = HashString(filename, 0x100);
            uint name2 = HashString(filename, 0x200);

            for (uint i = index; i < _mpqHashes.Length; ++i)
            {
                hash = _mpqHashes[i];
                if (hash.Name1 == name1 && hash.Name2 == name2)
                    return true;
            }

            for (uint i = 0; i < index; i++)
            {
                hash = _mpqHashes[i];
                if (hash.Name1 == name1 && hash.Name2 == name2)
                    return true;
            }

            hash = new MpqHash();
            return false;
        }

        /// <summary>
        /// Release the unmanaged and managed resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                if (disposing)
                {
                }

                ArchiveStream?.Dispose();
                _binaryReader?.Dispose();

                _isDisposed = true;
            }
        }

        private static uint[] BuildStormBuffer()
        {
            uint seed = 0x100001;

            uint[] result = new uint[0x500];

            for (uint index1 = 0; index1 < 0x100; index1++)
            {
                uint index2 = index1;
                for (int i = 0; i < 5; i++, index2 += 0x100)
                {
                    seed = ((seed * 125) + 3) % 0x2aaaab;
                    uint temp = (seed & 0xffff) << 16;
                    seed = ((seed * 125) + 3) % 0x2aaaab;

                    result[index2] = temp | (seed & 0xffff);
                }
            }

            return result;
        }

        private static void DecryptTable(Span<byte> data, string key)
        {
            DecryptBlock(data, HashString(key, 0x300));
        }
    }
}
