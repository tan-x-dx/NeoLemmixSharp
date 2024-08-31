using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp;

public sealed partial class NeoLemmixGame
{
    private void LoadResources()
    {
        var assembly = GetType().Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        LoadParticleResources();
        LoadBasherMask();
        LoadBomberMask();
        LoadFencerMask();
        LoadMinerMask();

        return;

        void LoadParticleResources()
        {
            var byteBuffer = ParticleHelper.GetByteBuffer();

            var byteCount = LoadByteData("particles.dat", byteBuffer);

            if (byteCount < byteBuffer.Length)
                throw new InvalidOperationException($"Read {byteCount} bytes from particles.dat - expected to read {byteBuffer.Length} bytes!");
        }

        [SkipLocalsInit]
        void LoadBasherMask()
        {
            Span<byte> byteBuffer = stackalloc byte[64];

            var byteCount = LoadByteData("basher_mask.dat", byteBuffer);

            TerrainMasks.InitialiseBasherMask(byteBuffer[..byteCount]);
        }

        [SkipLocalsInit]
        void LoadBomberMask()
        {
            Span<byte> byteBuffer = stackalloc byte[64];

            var byteCount = LoadByteData("bomber_mask.dat", byteBuffer);

            TerrainMasks.InitialiseBomberMask(byteBuffer[..byteCount]);
        }

        [SkipLocalsInit]
        void LoadFencerMask()
        {
            Span<byte> byteBuffer = stackalloc byte[64];

            var byteCount = LoadByteData("fencer_mask.dat", byteBuffer);

            TerrainMasks.InitialiseFencerMask(byteBuffer[..byteCount]);
        }

        [SkipLocalsInit]
        void LoadMinerMask()
        {
            Span<byte> byteBuffer = stackalloc byte[64];

            var byteCount = LoadByteData("miner_mask.dat", byteBuffer);

            TerrainMasks.InitialiseMinerMask(byteBuffer[..byteCount]);
        }

        int LoadByteData(string resourceName, Span<byte> byteBuffer)
        {
            var particleResourceName = Array.Find(resourceNames, s => s.EndsWith(resourceName));

            if (string.IsNullOrWhiteSpace(particleResourceName))
                throw new InvalidOperationException($"Could not load {resourceName}!");

            Stream? stream = null;
            BinaryReader? reader = null;

            int bytesRead;
            try
            {
                stream = assembly.GetManifestResourceStream(particleResourceName)!;
                reader = new BinaryReader(stream, Encoding.UTF8, false);

                if (byteBuffer.Length < stream.Length)
                    throw new InvalidOperationException($"Byte buffer size mismatch! Buffer size: {byteBuffer.Length}, file length: {stream.Length}");

                bytesRead = reader.Read(byteBuffer);
            }
            finally
            {
                reader?.Dispose();
                stream?.Dispose();
            }

            return bytesRead;
        }
    }
}