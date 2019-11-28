using System;
using System.Runtime.InteropServices;

namespace HSNXT.QuickAndDirtyGui
{
    public readonly struct UnmanagedValueType<T> : IDisposable where T : unmanaged
    {
        public int Size { get; }
        public IntPtr UnmanagedPointer { get; }

        private UnmanagedValueType(T structure, int size, bool deleteOld)
        {
            Size = size;
            UnmanagedPointer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, UnmanagedPointer, deleteOld);
        }

        public static unsafe UnmanagedValueType<T> Get(T structure, bool deleteOld = false)
            => new UnmanagedValueType<T>(structure, sizeof(T), deleteOld);

        public static UnmanagedValueType<T> GetMarshaled(T structure, bool deleteOld = false)
            => new UnmanagedValueType<T>(structure, Marshal.SizeOf<T>(), deleteOld);

        public void Dispose()
        {
            Marshal.DestroyStructure<T>(UnmanagedPointer);
            Marshal.FreeHGlobal(UnmanagedPointer);
        }

        public static unsafe implicit operator T*(UnmanagedValueType<T> from) => (T*) from.UnmanagedPointer;
    }
}