using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace HSNXT.QuickAndDirtyGui
{
    /// <summary>
    /// Represents an unmanaged array to a CLR-sized type.
    /// </summary>
    public readonly struct NativeArray : IDisposable
    {
        public Span<byte> Span => As<byte>();

        public IntPtr UnmanagedPointer { get; }
        public int Size { get; }

        public NativeArray(int size)
        {
            UnmanagedPointer = Marshal.AllocHGlobal(size);
            Size = size;
        }

        public unsafe Span<T> As<T>() where T : unmanaged => new Span<T>(UnmanagedPointer.ToPointer(), Size);

        public void Set<T>(T[] source) where T : unmanaged => new Span<T>(source).CopyTo(As<T>());
        public void Set<T>(Span<T> source) where T : unmanaged => source.CopyTo(As<T>());
        public void Set<T>(ReadOnlySpan<T> source) where T : unmanaged => source.CopyTo(As<T>());

        public static unsafe NativeArray Get<T>(int length) where T : unmanaged
        {
            return new NativeArray(length * sizeof(T));
        }

        public static NativeArray From<T>(T[] source) where T : unmanaged
        {
            var block = Get<T>(source.Length);
            block.Set(source);
            return block;
        }

        public static NativeArray From<T>(Span<T> source) where T : unmanaged
        {
            var block = Get<T>(source.Length);
            block.Set(source);
            return block;
        }

        public static NativeArray From<T>(ReadOnlySpan<T> source) where T : unmanaged
        {
            var block = Get<T>(source.Length);
            block.Set(source);
            return block;
        }

        public static unsafe NativeArray From(void* source, int length)
        {
            var block = Get<byte>(length);
            block.Set(new Span<byte>(source, length));
            return block;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(UnmanagedPointer);
        }
    }
}