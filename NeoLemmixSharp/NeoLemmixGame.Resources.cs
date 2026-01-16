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
            ParticleHelper.SetByteData(byteBuffer);
        }

        [SkipLocalsInit]
        void LoadTerrainMasks()
        {
            Span<byte> byteBuffer = stackalloc byte[256];

            var basherEraseMask = LoadMask("basher_mask.dat", byteBuffer, BasherAction.Instance);
            var bomberEraseMask = LoadMask("bomber_mask.dat", byteBuffer, ExploderAction.Instance);
            var fencerEraseMask = LoadMask("fencer_mask.dat", byteBuffer, FencerAction.Instance);
            //var lasererEraseMask = LoadMask("laserer_mask.dat", byteBuffer, LasererAction.Instance);
            var minerEraseMask = LoadMask("miner_mask.dat", byteBuffer, MinerAction.Instance);

            TerrainMasks.InitialiseTerrainMasks(
                basherEraseMask,
                bomberEraseMask,
                fencerEraseMask,
                null!,
                minerEraseMask);
        }

        TerrainEraseMask LoadMask(string maskName, Span<byte> byteBuffer, IDestructionMask destructionMask)
        {
            byteBuffer.Clear();
            var bytesRead = LoadByteData(maskName, byteBuffer);

            int i = 0;
            var maskSize = new Size(byteBuffer[i++], byteBuffer[i++]);

            int numberOfFrames = byteBuffer[i++];

            var anchorPosition = new Point(byteBuffer[i++], byteBuffer[i++]);

            byteBuffer = byteBuffer[i..];

            bytesRead -= i + 1;
            bytesRead |= 3;
            bytesRead++;

            byteBuffer = byteBuffer[..bytesRead];

            GeneratePositionData(
                maskSize,
                numberOfFrames,
                MemoryMarshal.Cast<byte, uint>(byteBuffer),
                out var ranges,
                out var positions);

            return new TerrainEraseMask(
                anchorPosition,
                maskSize,
                ranges,
                positions,
                destructionMask);
        }

        int LoadByteData(string resourceName, Span<byte> byteBuffer)
        {
            var particleResourceName = Array.Find(resourceNames, s => s.EndsWith(resourceName));

            if (string.IsNullOrWhiteSpace(particleResourceName))
                throw new InvalidOperationException($"Could not load {resourceName}!");

            using var stream = assembly.GetManifestResourceStream(particleResourceName)!;

            var byteLength = (int)stream.Length;

            if (byteBuffer.Length < byteLength)
                throw new InvalidOperationException($"Byte buffer size mismatch! Buffer size: {byteBuffer.Length}, file length: {byteLength}");

            stream.ReadExactly(byteBuffer[..byteLength]);

            return byteLength;
        }

        static void GeneratePositionData(
            Size maskSize,
            int numberOfFrames,
            ReadOnlySpan<uint> byteBuffer,
            out Range[] ranges,
            out Point[] positions)
        {
            var numberOfPixels = BitArrayHelpers.GetPopCount(byteBuffer);
            ranges = new Range[numberOfFrames];
            positions = new Point[numberOfPixels];

            var area = maskSize.Area();
            var bitEnumerator = new BitArrayEnumerator(byteBuffer);
            var r = 0;
            var p = 0;
            var startIndex = 0;
            int previousIndex = 0;
            int currentIndex = 0;

            while (bitEnumerator.MoveNext())
            {
                previousIndex = currentIndex;
                currentIndex = bitEnumerator.Current;

                positions[p++] = ConvertToPosition(currentIndex % area);

                if (previousIndex / area != currentIndex / area)
                {
                    ranges[r++] = new Range(startIndex, p - 1);
                    startIndex = p - 1;
                }
            }

            ranges[r++] = new Range(startIndex, numberOfPixels);

            Debug.Assert(r == ranges.Length);
            Debug.Assert(p == positions.Length);

            return;

            Point ConvertToPosition(int index)
            {
                var y = index / maskSize.W;
                var x = index - (y * maskSize.W);

                return new Point(x, y);
            }
        }
    }
}
