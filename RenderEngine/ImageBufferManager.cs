using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace RenderEngine
{
    public sealed unsafe class UnmanagedImageBuffer : IDisposable
    {
        private readonly IntPtr _bufferPtr;
        private readonly int _bufferSize;
        private readonly int _bytesPerPixel;

        public UnmanagedImageBuffer(int width, int height, int bytesPerPixel = 4)
        {
            Width = width;
            Height = height;
            _bytesPerPixel = bytesPerPixel;
            _bufferSize = width * height * bytesPerPixel;

            _bufferPtr = Marshal.AllocHGlobal(_bufferSize);
            Clear(0, 0, 0, 0);
        }

        public Span<byte> BufferSpan => new Span<byte>(_bufferPtr.ToPointer(), _bufferSize);

        public int Width { get; }
        public int Height { get; }

        public void Dispose()
        {
            if (_bufferPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_bufferPtr);
            }
        }

        private int GetPixelOffset(int x, int y)
        {
            return ((y * Width) + x) * _bytesPerPixel;
        }

        public void SetPixel(int x, int y, byte a, byte r, byte g, byte b)
        {
            var offset = GetPixelOffset(x, y);
            var buffer = BufferSpan;
            buffer[offset] = b;
            buffer[offset + 1] = g;
            buffer[offset + 2] = r;
            buffer[offset + 3] = a;
        }

        public void Clear(byte a, byte r, byte g, byte b)
        {
            var buffer = BufferSpan;

            var pixelVector = CreatePixelVector(a, r, g, b);

            var vectorSize = Vector<byte>.Count;
            var i = 0;

            for (; i <= buffer.Length - vectorSize; i += vectorSize)
            {
                pixelVector.CopyTo(buffer.Slice(i, vectorSize));
            }

            for (; i < buffer.Length; i += 4)
            {
                buffer[i] = b;
                buffer[i + 1] = g;
                buffer[i + 2] = r;
                buffer[i + 3] = a;
            }
        }

        private static Vector<byte> CreatePixelVector(byte a, byte r, byte g, byte b)
        {
            var pixelBytes = new byte[Vector<byte>.Count];
            for (var i = 0; i < pixelBytes.Length; i += 4)
            {
                pixelBytes[i] = b;
                pixelBytes[i + 1] = g;
                pixelBytes[i + 2] = r;
                pixelBytes[i + 3] = a;
            }

            return new Vector<byte>(pixelBytes);
        }

        // --------- New Methods -------------

        /// <summary>
        ///     Apply multiple pixel changes (x,y,color in BGRA format) directly to buffer.
        ///     No allocation, just writes into existing buffer.
        /// </summary>
        public void ApplyChanges(ReadOnlySpan<(int x, int y, uint bgra)> changes)
        {
            var buffer = BufferSpan;

            foreach (var (x, y, bgra) in changes)
            {
                if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
                {
                    continue;
                }

                var offset = GetPixelOffset(x, y);

                // BGRA is stored as bytes, but we have a uint bgra packed:
                // Decompose uint into bytes:
                buffer[offset] = (byte)(bgra & 0xFF); // B
                buffer[offset + 1] = (byte)((bgra >> 8) & 0xFF); // G
                buffer[offset + 2] = (byte)((bgra >> 16) & 0xFF); // R
                buffer[offset + 3] = (byte)((bgra >> 24) & 0xFF); // A
            }
        }

        /// <summary>
        ///     Replace entire unmanaged buffer with a new byte span (must be correct size).
        ///     Uses SIMD acceleration if available.
        /// </summary>
        public void ReplaceBuffer(ReadOnlySpan<byte> fullBuffer)
        {
            if (fullBuffer.Length != _bufferSize)
            {
                throw new ArgumentException("Input buffer size does not match.");
            }

            var buffer = BufferSpan;

            if (Avx2.IsSupported)
            {
                var vectorSize = 32; // 256 bits / 8
                var simdCount = _bufferSize / vectorSize;
                var remainder = _bufferSize % vectorSize;

                fixed (byte* srcPtr = fullBuffer)
                {
                    var dstPtr = (byte*)_bufferPtr;

                    for (var i = 0; i < simdCount; i++)
                    {
                        var vec = Avx.LoadVector256(srcPtr + (i * vectorSize));
                        Avx.Store(dstPtr + (i * vectorSize), vec);
                    }

                    // copy remaining bytes
                    for (var i = _bufferSize - remainder; i < _bufferSize; i++)
                    {
                        buffer[i] = fullBuffer[i];
                    }
                }
            }
            else
            {
                fullBuffer.CopyTo(buffer);
            }
        }

        /// <summary>
        ///     Returns a Span
        ///     <byte>
        ///         representing the pixel data for a horizontal run starting at (x,y).
        ///         Length is count pixels (count * bytesPerPixel bytes).
        ///         No bounds checking, so use carefully or add your own.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Span<byte> GetPixelSpan(int x, int y, int count)
        {
            if (x < 0 || y < 0 || x + count > Width || y >= Height)
            {
                throw new ArgumentOutOfRangeException();
            }

            var offset = GetPixelOffset(x, y);
            var length = count * _bytesPerPixel;
            return BufferSpan.Slice(offset, length);
        }
    }
}
