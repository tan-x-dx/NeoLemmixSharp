using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System;
using System.IO;
using System.Text;

namespace NeoLemmixSharp;

public sealed partial class NeoLemmixGame
{
    private void LoadResources()
    {
        var assembly = GetType().Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        LoadParticleResources();

        return;

        void LoadParticleResources()
        {
            var byteBuffer = ParticleHelper.GetByteBuffer();

            var byteCount = LoadByteData("particles.dat", byteBuffer);

            if (byteCount < byteBuffer.Length)
                throw new InvalidOperationException($"Read {byteCount} bytes from particles.dat - expected to read {byteBuffer.Length} bytes!");
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