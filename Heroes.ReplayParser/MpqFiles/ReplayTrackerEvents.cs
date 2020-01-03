using Heroes.MpqToolV2;
using Heroes.ReplayParser.Decoders;
using Heroes.ReplayParser.Replay;
using System;

namespace Heroes.ReplayParser.MpqFiles
{
    internal static class ReplayTrackerEvents
    {
        public static string FileName { get; } = "replay.tracker.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.LittleEndian;

            uint gameLoop = 0;

            TrackerEvent trackerEvent;

            while (BitReader.Index < source.Length)
            {
                gameLoop += new VersionedDecoder(source).ChoiceData!.GetValueAsUInt32();

                trackerEvent.TimeSpan = TimeSpan.FromSeconds(gameLoop / 16);
                trackerEvent.TrackerEventType = (TrackerEventType)new VersionedDecoder(source).GetValueAsUInt32();
                trackerEvent.VersionedDecoder = new VersionedDecoder(source);

                replay.TrackerEventsInternal.Add(trackerEvent);
            }
        }
    }
}
