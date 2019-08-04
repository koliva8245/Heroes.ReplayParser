using Ionic.BZip2;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Heroes.MpqTool
{
    public class MpqMemory : MpqBuffer
    {
        private readonly int _blockSize;

        private readonly MpqEntry _mpqEntry;
        private uint[] _blockPositions;

        private int _position;
        //private ReadOnlyMemory<byte> _currentData;
        private int _currentBlockIndex = -1;

        internal MpqMemory(MpqArchive archive, MpqEntry entry)
        {
            _position = 0;
            _mpqEntry = entry;

            _blockSize = archive.BlockSize;

            if (_mpqEntry.IsCompressed && !_mpqEntry.IsSingleUnit)
                LoadBlockPositions();

            if (_mpqEntry.IsSingleUnit && Buffer.IsEmpty)
                LoadSingleUnit(archive);

            Index = 0;
        }

        public override int Length => (int)_mpqEntry.FileSize;

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
            switch (compressionType[0])
            {
                //case 1: // Huffman
                //    return MpqHuffman.Decompress(sinput).ToArray();
                //case 2: // ZLib/Deflate
                //    return ZlibDecompress(sinput, outputLength);
                //case 8: // PKLib/Impode
                //    return PKDecompress(sinput, outputLength);
                case 0x10: // BZip2
                    return BZip2Decompress(streamInput, outputLength);
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
                default:
                    throw new MpqParserException("Compression is not yet supported: 0x" + compressionType[0].ToString("X"));
            }
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
            int blockPositionCount = (int)((_mpqEntry.FileSize + _blockSize - 1) / _blockSize) + 1;

            // Files with metadata have an extra block containing block checksums
            if ((_mpqEntry.Flags & MpqFileFlags.FileHasMetadata) != 0)
                blockPositionCount++;

            _blockPositions = new uint[blockPositionCount];

            Index = (int)_mpqEntry.FilePosition;

            for (int i = 0; i < blockPositionCount; i++)
                _blockPositions[i] = ReadUInt32();

            uint blockpossize = (uint)blockPositionCount * 4;

            /*
            if(_blockPositions[0] != blockpossize)
                _entry.Flags |= MpqFileFlags.Encrypted;
             */

            if (_mpqEntry.IsEncrypted)
            {
                if (_mpqEntry.EncryptionSeed == 0) // This should only happen when the file name is not known
                {
                    _mpqEntry.EncryptionSeed = MpqArchive.DetectFileSeed(_blockPositions[0], _blockPositions[1], blockpossize) + 1;
                    if (_mpqEntry.EncryptionSeed == 1)
                        throw new MpqParserException("Unable to determine encyption seed");
                }

                MpqArchive.DecryptBlock(_blockPositions, _mpqEntry.EncryptionSeed - 1);

                if (_blockPositions[0] != blockpossize)
                    throw new MpqParserException("Decryption failed");
                if (_blockPositions[1] > _blockSize + blockpossize)
                    throw new MpqParserException("Decryption failed");
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

        private void LoadSingleUnit(MpqArchive mpqArchive)
        {
            Index = (int)_mpqEntry.FilePosition;

            mpqArchive.MpqBuffer.Index = Index;
            ReadOnlySpan<byte> fileData = mpqArchive.MpqBuffer.ReadBytes((int)_mpqEntry.CompressedSize).Span;
            //ReadOnlySpan<byte> fileData = ReadBytes((int)_mpqEntry.CompressedSize).Span;

            if (_mpqEntry.CompressedSize == _mpqEntry.FileSize)
            {
               // _currentData = filedata;
            }
            else
                Buffer = DecompressMulti(fileData, (int)_mpqEntry.FileSize);
        }
    }
}
