using System;
using System.Runtime.InteropServices;

namespace HSNXT.QuickAndDirtyGui
{
    public readonly struct UnmanagedBlock : IDisposable
    {
        public int Size { get; }
        public IntPtr UnmanagedPointer { get; }

        public UnmanagedBlock(int size)
        {
            Size = size;
            UnmanagedPointer = Marshal.AllocHGlobal(size);
        }

        public unsafe UnmanagedBlock Get<T>(int length) where T : unmanaged
            => new UnmanagedBlock(length * sizeof(T));

        public UnmanagedBlock GetMarshaled<T>(int length) where T : unmanaged
            => new UnmanagedBlock(length * Marshal.SizeOf<T>());

        public void Set(int[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);
        public void Set(char[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);
        public void Set(short[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);
        public void Set(long[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);
        public void Set(float[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);
        public void Set(double[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);
        public void Set(byte[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);
        public void Set(IntPtr[] source) => Marshal.Copy(source, 0, UnmanagedPointer, source.Length);

        public static UnmanagedBlock From(int[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }
        public static UnmanagedBlock From(char[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }
        public static UnmanagedBlock From(short[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }
        public static UnmanagedBlock From(long[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }
        public static UnmanagedBlock From(float[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }
        public static UnmanagedBlock From(double[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }
        public static UnmanagedBlock From(byte[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }
        public static UnmanagedBlock From(IntPtr[] source)
        {
            var block = new UnmanagedBlock(source.Length);
            block.Set(source);
            return block;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(UnmanagedPointer);
        }
    }
}