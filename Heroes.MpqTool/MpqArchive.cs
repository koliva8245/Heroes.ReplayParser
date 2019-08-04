using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;

namespace Heroes.MpqTool
{
    public class MpqArchive
    {
        private static readonly uint[] _stormBuffer;
        private MpqHeader? _mpqHeader;
        private MpqHash[]? _mpqHashes;
        private MpqEntry[]? _mpqEntries;

        private long _headerOffset;

        static MpqArchive()
        {
            _stormBuffer = BuildStormBuffer();
        }

        public MpqArchive(string fileName)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                Memory<byte> fileStreamBuffer = new byte[fileStream.Length];
                fileStream.Read(fileStreamBuffer.Span);

                MpqBuffer = new MpqBuffer(fileStreamBuffer);
            }

            try
            {
                Initialize();
            }
            catch
            {
                throw;
            }
        }

        public int BlockSize { get; private set; }
        internal MpqBuffer MpqBuffer { get; private set; }

        public MpqMemory OpenFile(string fileName)
        {
            MpqEntry entry;

            if (!TryGetHashEntry(fileName, out MpqHash hash))
                throw new FileNotFoundException("File not found: " + fileName);

            if (_mpqEntries == null)
                throw new MpqParserException($"{nameof(_mpqEntries)} is null");

            entry = _mpqEntries[hash.BlockIndex];
            if (string.IsNullOrWhiteSpace(fileName))
                entry.FileName = fileName.AsMemory();

            return new MpqMemory(this, entry);
        }

        public MpqMemory OpenFile(MpqEntry entry)
        {
            return new MpqMemory(this, entry);
        }

        public bool AddListfileFilenames()
        {
            if (!AddFileName("(listfile)")) return false;

            AddFilenames(OpenFile("(listfile)"));

            return true;
        }

        public bool AddFileName(string filename)
        {
            if (!TryGetHashEntry(filename, out MpqHash hash)) return false;

            if (_mpqEntries == null)
                throw new MpqParserException($"{nameof(_mpqEntries)} is null");

            _mpqEntries[hash.BlockIndex].FileName = filename.AsMemory();
            return true;
        }

        public bool AddFilename(ReadOnlyMemory<char> filename)
        {
            if (!TryGetHashEntry(filename.Span, out MpqHash hash)) return false;

            if (_mpqEntries == null)
                throw new MpqParserException($"{nameof(_mpqEntries)} is null");

            _mpqEntries[hash.BlockIndex].FileName = filename;
            return true;
        }

        // Used for Hash Tables and Block Tables
        internal static void DecryptTable(Span<byte> data, string key)
        {
            DecryptBlock(data, HashString(key, 0x300));
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

        internal static uint HashString(ReadOnlySpan<char> input, int offset)
        {
            Span<char> upperInput = new char[input.Length];

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

        private void Initialize()
        {
            if (LocateMpqHeader() == false)
                throw new MpqParserException("Unable to find MPQ header");

            if (_mpqHeader?.HashTableOffsetHigh != 0 || _mpqHeader.ExtendedBlockTableOffset != 0 || _mpqHeader.BlockTableOffsetHigh != 0)
                throw new MpqParserException("MPQ format version 1 features are not supported");

            BlockSize = 0x200 << _mpqHeader.BlockSize;

            // Load hash table
            MpqBuffer.Index = (int)_mpqHeader.HashTablePos;
            ReadOnlyMemory<byte> hashData = MpqBuffer.ReadBytes((int)(_mpqHeader.HashTableSize * MpqHash.Size));
            DecryptTable(MemoryMarshal.AsMemory(hashData).Span, "(hash table)");

            MpqBuffer tempDataBuffer = new MpqBuffer(hashData);
            _mpqHashes = new MpqHash[_mpqHeader.HashTableSize];

            for (int i = 0; i < _mpqHeader.HashTableSize; i++)
                _mpqHashes[i] = new MpqHash(tempDataBuffer);

            // Load entry table
            MpqBuffer.Index = (int)_mpqHeader.BlockTablePos;
            ReadOnlyMemory<byte> entryData = MpqBuffer.ReadBytes((int)(_mpqHeader.BlockTableSize * MpqHash.Size));
            DecryptTable(MemoryMarshal.AsMemory(entryData).Span, "(block table)");

            tempDataBuffer = new MpqBuffer(entryData);
            _mpqEntries = new MpqEntry[_mpqHeader.BlockTableSize];

            for (int i = 0; i < _mpqHeader.BlockTableSize; i++)
                _mpqEntries[i] = new MpqEntry(tempDataBuffer, (uint)_headerOffset);
        }

        private bool LocateMpqHeader()
        {
            for (int i = 0; i < MpqBuffer.Buffer.Length - MpqHeader.Size; i += 0x200)
            {
                MpqBuffer.Index = i;
                _mpqHeader = MpqHeader.FromBuffer(MpqBuffer);

                if (_mpqHeader != null)
                {
                    _headerOffset = i;
                    _mpqHeader.SetHeaderOffset(_headerOffset);

                    return true;
                }
            }

            return false;
        }

        private bool TryGetHashEntry(ReadOnlySpan<char> filename, out MpqHash hash)
        {
            if (_mpqHeader == null || _mpqHashes == null)
                throw new MpqParserException($"{nameof(_mpqHeader)} or {nameof(_mpqHashes)} is null");

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

        private void AddFilenames(MpqBuffer mpqBuffer)
        {
            while (!mpqBuffer.IsEndOfBuffer)
                AddFilename(mpqBuffer.ReadStringAsMemory());
        }
    }
}
