using System;
using System.Runtime.InteropServices;

namespace HSNXT.QuickAndDirtyGui
{
    public readonly struct NativeStruct<T> : IDisposable where T : unmanaged
    {
        public IntPtr UnmanagedPointer { get; }
        public int Size { get; }

        private NativeStruct(T structure, int size, bool deleteOld)
        {
            UnmanagedPointer = Marshal.AllocHGlobal(size);
            Size = size;
            Marshal.StructureToPtr(structure, UnmanagedPointer, deleteOld);
        }

        public static unsafe NativeStruct<T> Get(T structure, bool deleteOld = false)
            => new NativeStruct<T>(structure, sizeof(T), deleteOld);

        public static NativeStruct<T> GetMarshaled(T structure, bool deleteOld = false)
            => new NativeStruct<T>(structure, Marshal.SizeOf<T>(), deleteOld);

        public void Dispose()
        {
            Marshal.DestroyStructure<T>(UnmanagedPointer);
            Marshal.FreeHGlobal(UnmanagedPointer);
        }

        public static unsafe implicit operator T*(NativeStruct<T> from) => (T*) from.UnmanagedPointer;
    }
}