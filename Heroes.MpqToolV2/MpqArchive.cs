﻿using Ionic.BZip2;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Heroes.MpqToolV2
{
    public class MpqArchive : IDisposable
    {
        private static readonly uint[] _stormBuffer;

        private readonly Stream _archiveStream;
        private readonly MpqHeader _mpqHeader;

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
            _archiveStream = stream ?? throw new ArgumentNullException(nameof(stream));

            Span<byte> headerBuffer = stackalloc byte[2048]; // guess how much the header will be
            stream.Read(headerBuffer);

            BitReader.ResetIndex();
            _mpqHeader = new MpqHeader(headerBuffer);

            if (_mpqHeader.HashTableOffsetHigh != 0 || _mpqHeader.ExtendedBlockTableOffset != 0 || _mpqHeader.BlockTableOffsetHigh != 0)
                throw new MpqToolException("MPQ format version 1 features are not supported");

            // LoadHashTable
            Span<byte> hashBuffer = stackalloc byte[(int)(_mpqHeader.HashTableSize * MpqHash.Size)]; // get the hash table buffer
            stream.Position = (int)_mpqHeader.HashTablePos;
            stream.Read(hashBuffer);

            DecryptTable(hashBuffer, "(hash table)");

            _mpqHashes = new MpqHash[_mpqHeader.HashTableSize];

            BitReader.ResetIndex();
            for (int i = 0; i < _mpqHeader.HashTableSize; i++)
                _mpqHashes[i] = new MpqHash(hashBuffer);

            // LoadEntryTable;
            Span<byte> entryBuffer = stackalloc byte[(int)(_mpqHeader.BlockTableSize * MpqArchiveEntry.Size)]; // get the entry table buffer
            stream.Position = (int)_mpqHeader.BlockTablePos;
            stream.Read(entryBuffer);

            DecryptTable(entryBuffer, "(block table)");

            _mpqArchiveEntries = new MpqArchiveEntry[_mpqHeader.BlockTableSize];

            BitReader.ResetIndex();
            for (int i = 0; i < _mpqHeader.BlockTableSize; i++)
                _mpqArchiveEntries[i] = new MpqArchiveEntry(entryBuffer, (uint)_mpqHeader.HeaderOffset);
        }

        public ReadOnlySpan<byte> GetHeaderBytes(int size = 0x100)
        {
            Span<byte> data = new byte[size];
            _archiveStream.Position = 0;
            _archiveStream.Read(data);

            return data;
        }

        public ReadOnlySpan<byte> OpenFile(string filename)
        {
            if (!TryGetHashEntry(filename, out MpqHash hash))
                throw new FileNotFoundException("File not found: " + filename);

            MpqArchiveEntry entry = _mpqArchiveEntries[hash.BlockIndex];
            if (entry.FileName == null)
                entry.FileName = filename;

            return DecompressEntry(entry);
        }

        public bool AddListfileFileNames()
        {
            if (!AddFileName("(listfile)"))
                return false;

            AddFilenames(OpenFile("(listfile)"));

            return true;
        }

        public void AddFilenames(ReadOnlySpan<byte> source)
        {
            int startIndex = 0;
            int index = 0;

            do
            {
                byte charByte = source[index];

                // \n - UNIX   \r\n - DOS   \r - Mac
                if (charByte == 10 || charByte == 13)
                {
                    Span<char> data = stackalloc char[index - startIndex];

                    Encoding.UTF8.GetChars(source.Slice(startIndex, index - startIndex), data);

                    // if it's a \r, check one ahead for a \n
                    if (charByte == 13 && index < source.Length)
                    {
                        byte nByte = source[index + 1];
                        if (nByte == 10)
                        {
                            index++;
                        }
                    }

                    index++;

                    AddFileName(data.ToString());
                    startIndex = index;
                }

                index++;
            } while (index < source.Length);
        }

        public bool AddFileName(string fileName)
        {
            if (!TryGetHashEntry(fileName, out MpqHash hash)) return false;

            _mpqArchiveEntries[hash.BlockIndex].FileName = fileName;
            return true;
        }

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
                    _archiveStream?.Dispose();
                }

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

        private static void DecryptBlock(Span<byte> data, uint seed1)
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

        private static void DecryptTable(Span<byte> data, string key)
        {
            DecryptBlock(data, HashString(key, 0x300));
        }

        private static void DecompressByteData(Span<byte> buffer, int compressedSize)
        {
            byte compressionType = buffer[0];

            switch (compressionType)
            {
                case 1: // Huffman
                    throw new MpqToolException("Huffman not yet supported");
                case 2: // ZLib/Deflate
                    throw new MpqToolException("ZLib/Deflate not yet supported");
                case 8: // PKLib/Impode
                    throw new MpqToolException("PKLib/Impode not yet supported");
                case 0x10:
                    BZip2Decompress(buffer, compressedSize);
                    break;
                case 0x80: // IMA ADPCM Stereo
                    throw new MpqToolException("IMA ADPCM Stereo not yet supported");
                case 0x40: // IMA ADPCM Mono
                    throw new MpqToolException("IMA ADPCM Mono not yet supported");
                case 0x12:
                    throw new MpqToolException("LZMA not yet supported");

                // Combos
                case 0x22:
                    // TODO: sparse then zlib
                    throw new MpqToolException("Not yet supported");
                case 0x30:
                    // TODO: sparse then bzip2
                    throw new MpqToolException("Not yet supported");
                case 0x41:
                    throw new MpqToolException("Not yet supported");
                case 0x48:
                    throw new MpqToolException("Not yet supported");
                case 0x81:
                    throw new MpqToolException("Not yet supported");
                case 0x88:
                    throw new MpqToolException("Not yet supported");
                default:
                    throw new MpqToolException("Compression is not yet supported: 0x" + compressionType.ToString("X"));
            }
        }

        private static void BZip2Decompress(Span<byte> buffer, int compressedSize)
        {
            using MemoryStream memoryStream = new MemoryStream(buffer.Slice(1, compressedSize).ToArray());
            using BZip2InputStream stream = new BZip2InputStream(memoryStream);

            stream.Read(buffer);
        }

        private ReadOnlySpan<byte> DecompressEntry(MpqArchiveEntry mpqArchiveEntry)
        {
            byte[] fileData = new byte[(int)mpqArchiveEntry.FileSize];

            _archiveStream.Position = mpqArchiveEntry.FilePosition;

            int read = _archiveStream.Read(fileData, 0, (int)mpqArchiveEntry.CompressedSize);

            if (read != mpqArchiveEntry.CompressedSize)
                throw new MpqToolException("Insufficient data or invalid data length");

            if (mpqArchiveEntry.CompressedSize == mpqArchiveEntry.FileSize)
            {
                return fileData;
            }
            else
            {
                DecompressByteData(fileData, (int)mpqArchiveEntry.CompressedSize);
            }

            return fileData;
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
    }
}
