using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp;

public sealed partial class NeoLemmixGame
{
    private void LoadResources()
    {
        var assembly = GetType().Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        LoadParticleResources();
        LoadTerrainMasks();

        return;

        void LoadParticleResources()
        {
            var byteBuffer = ParticleHelper.GetByteBuffer();

            var bytesRead = LoadByteData("particles.dat", byteBuffer);

            Debug.Assert(bytesRead == byteBuffer.Length);
        }

        [SkipLocalsInit]
        void LoadTerrainMasks()
        {
            Span<byte> byteBuffer = stackalloc byte[256];

            var basherEraseMask = LoadMask("basher_mask.dat", byteBuffer);
            var bomberEraseMask = LoadMask("bomber_mask.dat", byteBuffer);
            var fencerEraseMask = LoadMask("fencer_mask.dat", byteBuffer);
            //var lasererEraseMask = LoadMask("laserer_mask.dat", byteBuffer);
            var minerEraseMask = LoadMask("miner_mask.dat", byteBuffer);

            TerrainMasks.InitialiseTerrainMasks(
                basherEraseMask,
                bomberEraseMask,
                fencerEraseMask,
                null!,
                minerEraseMask);
        }

        TerrainEraseMask LoadMask(string maskName, Span<byte> byteBuffer)
        {
            byteBuffer.Clear();
            var bytesRead = LoadByteData(maskName, byteBuffer);

            int i = 0;
            int maskWidth = byteBuffer[i++];
            int maskHeight = byteBuffer[i++];
            int numberOfFrames = byteBuffer[i++];

            int anchorPointX = byteBuffer[i++];
            int anchorPointY = byteBuffer[i++];

            byteBuffer = byteBuffer[i..];

            bytesRead -= i + 1;
            bytesRead |= 3;
            bytesRead++;

            byteBuffer = byteBuffer[..bytesRead];

            GenerateLevelPositionData(
                maskWidth,
                maskHeight,
                numberOfFrames,
                MemoryMarshal.Cast<byte, uint>(byteBuffer),
                out var ranges,
                out var levelPositions);

            return new TerrainEraseMask(
                maskWidth,
                maskHeight,
                new LevelPosition(anchorPointX, anchorPointY),
                ranges,
                levelPositions,
                BasherAction.Instance);
        }

        int LoadByteData(string resourceName, Span<byte> byteBuffer)
        {
            var particleResourceName = Array.Find(resourceNames, s => s.EndsWith(resourceName));

            if (string.IsNullOrWhiteSpace(particleResourceName))
                throw new InvalidOperationException($"Could not load {resourceName}!");

            var byteLength = 0;

            using var stream = assembly.GetManifestResourceStream(particleResourceName)!;

            byteLength = (int)stream.Length;

            if (byteBuffer.Length < byteLength)
                throw new InvalidOperationException($"Byte buffer size mismatch! Buffer size: {byteBuffer.Length}, file length: {byteLength}");

            stream.ReadExactly(byteBuffer[..byteLength]);

            return byteLength;
        }

        static void GenerateLevelPositionData(
            int maskWidth,
            int maskHeight,
            int numberOfFrames,
            ReadOnlySpan<uint> byteBuffer,
            out Range[] ranges,
            out LevelPosition[] levelPositions)
        {
            var numberOfPixels = BitArrayHelpers.GetPopCount(byteBuffer);
            ranges = new Range[numberOfFrames];
            levelPositions = new LevelPosition[numberOfPixels];

            var area = maskWidth * maskHeight;
            var bitEnumerator = new BitArrayEnumerator(byteBuffer, numberOfPixels);
            var r = 0;
            var p = 0;
            var startIndex = 0;
            int previousIndex = 0;
            int currentIndex = 0;

            while (bitEnumerator.MoveNext())
            {
                previousIndex = currentIndex;
                currentIndex = bitEnumerator.Current;

                levelPositions[p++] = ConvertToPosition(currentIndex % area);

                if (previousIndex / area != currentIndex / area)
                {
                    ranges[r++] = new Range(startIndex, p - 1);
                    startIndex = p - 1;
                }
            }

            ranges[r++] = new Range(startIndex, numberOfPixels);

            Debug.Assert(r == ranges.Length);
            Debug.Assert(p == levelPositions.Length);

            return;

            LevelPosition ConvertToPosition(int index)
            {
                var y = index / maskWidth;
                var x = index - (y * maskWidth);

                return new LevelPosition(x, y);
            }
        }
    }
}