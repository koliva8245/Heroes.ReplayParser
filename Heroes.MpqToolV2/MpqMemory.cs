using Ionic.BZip2;
using System;
using System.IO;

namespace Heroes.MpqToolV2
{
    public class MpqMemory : MpqBuffer
    {
        private readonly int _blockSize;

        private readonly MpqArchiveEntry _mpqArchiveEntry;

        private uint[] _blockPositions;
        private int _position;
        //private ReadOnlyMemory<byte> _currentData;
        private int _currentBlockIndex = -1;

        internal MpqMemory(ReadOnlyMemory<byte> data)
        {
            Buffer = data;
        }

        internal MpqMemory(MpqArchive archive, MpqArchiveEntry mpqArchiveEntry)
        {
            _position = 0;
            _mpqArchiveEntry = mpqArchiveEntry;

            _blockSize = archive.BlockSize;

            if (_mpqArchiveEntry.IsCompressed && !_mpqArchiveEntry.IsSingleUnit)
                LoadBlockPositions();

            if (_mpqArchiveEntry.IsSingleUnit && Buffer.IsEmpty)
                LoadSingleUnit(archive.ArchiveStream);

            Index = 0;
        }

        public override int Length => (int)_mpqArchiveEntry.FileSize;

        //public override ReadOnlyMemory<char> ReadStringAsMemory()
        //{
        //    //if (_mpqEntry.IsSingleUnit)
        //    //{
        //    //    if (_currentData.IsEmpty)
        //    //        LoadSingleUnit();
        //    //}

        //    //int bytestocopy = Math.Min(_currentData.Length - _position, count);

        //    //MpqBuffer mpqBuffer = new MpqBuffer(_currentData);
        //   // return base.ReadStringAsMemory();
        //}

        private static ReadOnlyMemory<byte> DecompressMulti(ReadOnlySpan<byte> input, int outputLength)
        {
            ReadOnlySpan<byte> compressionType = input.Slice(0, 1);

            using Stream streamInput = new MemoryStream(input.Slice(1).ToArray());

            // WC3 onward mosly use Zlib
            // Starcraft 1 mostly uses PKLib, plus types 41 and 81 for audio files
            return compressionType[0] switch
            {
                //case 1: // Huffman
                //    return MpqHuffman.Decompress(sinput).ToArray();
                //case 2: // ZLib/Deflate
                //    return ZlibDecompress(sinput, outputLength);
                //case 8: // PKLib/Impode
                //    return PKDecompress(sinput, outputLength);
                0x10 => BZip2Decompress(streamInput, outputLength),
                //case 0x80: // IMA ADPCM Stereo
                //    return MpqWavCompression.Decompress(sinput, 2);
                //case 0x40: // IMA ADPCM Mono
                //    return MpqWavCompression.Decompress(sinput, 1);

                //case 0x12:
                //    // TODO: LZMA
                //    throw new MpqParserException("LZMA compression is not yet supported");

                //// Combos
                //case 0x22:
                //    // TODO: sparse then zlib
                //    throw new MpqParserException("Sparse compression + Deflate compression is not yet supported");
                //case 0x30:
                //    // TODO: sparse then bzip2
                //    throw new MpqParserException("Sparse compression + BZip2 compression is not yet supported");
                //case 0x41:
                //    sinput = MpqHuffman.Decompress(sinput);
                //    return MpqWavCompression.Decompress(sinput, 1);
                //case 0x48:
                //    {
                //        byte[] result = PKDecompress(sinput, outputLength);
                //        return MpqWavCompression.Decompress(new MemoryStream(result), 1);
                //    }
                //case 0x81:
                //    sinput = MpqHuffman.Decompress(sinput);
                //    return MpqWavCompression.Decompress(sinput, 2);
                //case 0x88:
                //    {
                //        byte[] result = PKDecompress(sinput, outputLength);
                //        return MpqWavCompression.Decompress(new MemoryStream(result), 2);
                //    }
                _ => throw new MpqToolException("Compression is not yet supported: 0x" + compressionType[0].ToString("X")),
            };
        }

        private static ReadOnlyMemory<byte> BZip2Decompress(Stream data, int expectedLength)
        {
            Memory<byte> output = new byte[expectedLength];

            using (BZip2InputStream stream = new BZip2InputStream(data))
            {
                stream.Read(output.Span);
            }

            return output;
        }

        // Compressed files start with an array of offsets to make seeking possible
        private void LoadBlockPositions()
        {
            int blockPositionCount = (int)((_mpqArchiveEntry.FileSize + _blockSize - 1) / _blockSize) + 1;

            // Files with metadata have an extra block containing block checksums
            if ((_mpqArchiveEntry.Flags & MpqFileFlags.FileHasMetadata) != 0)
                blockPositionCount++;

            _blockPositions = new uint[blockPositionCount];

            Index = (int)_mpqArchiveEntry.FilePosition;

            for (int i = 0; i < blockPositionCount; i++)
                _blockPositions[i] = ReadUInt32();

            uint blockpossize = (uint)blockPositionCount * 4;

            /*
            if(_blockPositions[0] != blockpossize)
                _entry.Flags |= MpqFileFlags.Encrypted;
             */

            if (_mpqArchiveEntry.IsEncrypted)
            {
                if (_mpqArchiveEntry.EncryptionSeed == 0) // This should only happen when the file name is not known
                {
                    _mpqArchiveEntry.EncryptionSeed = MpqArchive.DetectFileSeed(_blockPositions[0], _blockPositions[1], blockpossize) + 1;
                    if (_mpqArchiveEntry.EncryptionSeed == 1)
                        throw new MpqToolException("Unable to determine encyption seed");
                }

                MpqArchive.DecryptBlock(_blockPositions, _mpqArchiveEntry.EncryptionSeed - 1);

                if (_blockPositions[0] != blockpossize)
                    throw new MpqToolException("Decryption failed");
                if (_blockPositions[1] > _blockSize + blockpossize)
                    throw new MpqToolException("Decryption failed");
            }
        }

        // SingleUnit entries can be compressed but are never encrypted
        //private int ReadInternalSingleUnit(byte[] buffer, int offset, int count)
        //{
        //    if (_position >= Length)
        //        return 0;

        //    if (_currentData == null)
        //        LoadSingleUnit();

        //    int bytestocopy = Math.Min((int)(_currentData.Length - _position), count);

        //    Array.Copy(_currentData, _position, buffer, offset, bytestocopy);

        //    _position += bytestocopy;
        //    return bytestocopy;
        //}

        private void LoadSingleUnit(Stream stream)
        {
            Index = (int)_mpqArchiveEntry.FilePosition;

            // Read the entire file into memory
            Span<byte> fileData = stackalloc byte[(int)_mpqArchiveEntry.CompressedSize];

            // byte[] filedata = new byte[_mpqArchiveEntry.CompressedSize];
            lock (stream)
            {
                stream.Seek(_mpqArchiveEntry.FilePosition, SeekOrigin.Begin);
                int read = stream.Read(fileData);
                if (read != fileData.Length)
                    throw new MpqToolException("Insufficient data or invalid data length");
            }

            if (_mpqArchiveEntry.CompressedSize == _mpqArchiveEntry.FileSize)
            {
                //_currentData = fileData;
            }
            else
            {
                Buffer = DecompressMulti(fileData, (int)_mpqArchiveEntry.FileSize);
            }

            //Index = (int)_mpqArchiveEntry.FilePosition;

            //mpqArchive.MpqBuffer.Index = Index;
            //ReadOnlySpan<byte> fileData = mpqArchive.MpqBuffer.ReadBytes((int)_mpqArchiveEntry.CompressedSize).Span;
            ////ReadOnlySpan<byte> fileData = ReadBytes((int)_mpqEntry.CompressedSize).Span;

            //if (_mpqArchiveEntry.CompressedSize == _mpqArchiveEntry.FileSize)
            //{
            //   // _currentData = filedata;
            //}
            //else
            //    Buffer = DecompressMulti(fileData, (int)_mpqArchiveEntry.FileSize);
        }
    }
}
