using Serilog;
using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Compression.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ChunkHeader
{
    public static ILogger Logger => Log.Logger
        .ForContext(Log.Category, Log.Compression)
        .ForContext<ChunkHeader>();
    
    public const ulong ArchiveHeaderTag = 0x222222229E2A83C1;
    public const byte CompressorId = 0x3;
    public const ulong FullChunkSize = 0x20000;

    public ulong HeaderTag;
    public ulong ChunkSize;
    public byte Compressor;
    public ulong CompressedSize1;
    public ulong DecompressedSize1;
    public ulong CompressedSize2;
    public ulong DecompressedSize2;

    public readonly void DumpDebug(int chunk = -1)
    {
        Logger.Verbose("Dumping ChunkHeader");
        if (chunk != -1)
        {
            Logger.Verbose($"Chunk {chunk}");
        }
        Logger.Verbose($"HeaderTag {HeaderTag:X8}");
        Logger.Verbose($"ChunkSize {ChunkSize}");
        Logger.Verbose($"Compressor {Compressor}");
        Logger.Verbose($"CompressedSize1 {CompressedSize1}");
        Logger.Verbose($"DecompressedSize1 {DecompressedSize1}");
        Logger.Verbose($"CompressedSize2 {CompressedSize2}");
        Logger.Verbose($"DecompressedSize2 {DecompressedSize2}");
        Logger.Verbose("--");

    }

    internal readonly void Validate(bool isEof, int chunkCounter)
    {
        if (HeaderTag != ArchiveHeaderTag)
        {
            throw new InvalidOperationException("bad chunk header tag");
        }
        if (Compressor != CompressorId)
        {
            throw new InvalidOperationException("bad compressor id");
        }
        if (DecompressedSize1 != DecompressedSize2)
        {
            Logger.Warning("Decompressed size mismatch in chunk {chunk}: {DecompressedSize1} vs {DecompressedSize2}", chunkCounter, DecompressedSize1, DecompressedSize2);
        }
        if (CompressedSize1 != CompressedSize2)
        {
            Logger.Warning("Compressed size mismatch in chunk {chunk}: {CompressedSize1} vs {CompressedSize2}", chunkCounter, CompressedSize1, CompressedSize2);
        }
        if (ChunkSize != FullChunkSize)
        {
            Logger.Warning("Unexpected ChunkSize in chunk {chunk}: {ChunkSize} vs {ExpectedChunkSize}", chunkCounter, ChunkSize, FullChunkSize);
        }
        if (!isEof && ChunkSize != DecompressedSize1)
        {
            Logger.Warning("Unexpected DecompressedSize1 in chunk {chunk}: {ChunkSize} vs {DecompressedSize1}", chunkCounter, ChunkSize, DecompressedSize1);
        }
        if (!isEof && ChunkSize != DecompressedSize2)
        {
            Logger.Warning("Unexpected DecompressedSize2 in chunk {chunk}: {ChunkSize} vs {DecompressedSize2}", chunkCounter, ChunkSize, DecompressedSize2);
        }
    }
}
