using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;

namespace HSNXT.QuickAndDirtyGui
{
    /*
                   |           x64           |           x86          
                   |   Marshal.              |   Marshal.             
        Primitive  | SizeOf<T>()  sizeof(T)  | SizeOf<T>()  sizeof(T) 
        =========  | =========== =========== | =========== ===========
          Boolean  |     4     <->    1      |      4    <->    1     
             Byte  |     1            1      |      1           1     
            SByte  |     1            1      |      1           1     
            Int16  |     2            2      |      2           2     
           UInt16  |     2            2      |      2           2     
            Int32  |     4            4      |      4           4     
           UInt32  |     4            4      |      4           4     
            Int64  |     8            8      |      8           8     
           UInt64  |     8            8      |      8           8     
           IntPtr  |     8            8     <->     4           4     
          UIntPtr  |     8            8     <->     4           4     
             Char  |     1     <->    2      |      1    <->    2     
           Double  |     8            8      |      8           8     
           Single  |     4            4      |      4           4
     */
    
    public static class Helpers
    {
        public static UnmanagedValueType<T> ToPtrClr<T>(this T structure, bool deleteOld = false) where T : unmanaged
            => UnmanagedValueType<T>.Get(structure, deleteOld);

        public static UnmanagedValueType<T> ToPtrMarshal<T>(this T structure, bool deleteOld = false) where T : unmanaged
            => UnmanagedValueType<T>.GetMarshaled(structure, deleteOld);

        public static UnmanagedBlock ToArrayPtrClr(this int[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(int) * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrClr(this short[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(short) * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrClr(this long[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(long) * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrClr(this float[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(float) * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrClr(this double[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(double) * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrClr(this byte[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(byte) * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrClr(this IntPtr[] arr)
        {
            var memory = new UnmanagedBlock(IntPtr.Size * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrAnsi(this char[] arr)
        {
            var memory = new UnmanagedBlock(Marshal.SystemDefaultCharSize * arr.Length);
            memory.Set(arr);
            return memory;
        }

        public static UnmanagedBlock ToArrayPtrWide(this char[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(char) * arr.Length);
            for (var i = 0; i < arr.Length; i++)
            {
                Marshal.WriteInt16(memory.UnmanagedPointer, i * sizeof(char), arr[i]);
            }

            return memory;
        }

        public static UnmanagedBlock ToArrayPtr8(this bool[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(bool) * arr.Length);
            for (var i = 0; i < arr.Length; i++)
            {
                Marshal.WriteByte(memory.UnmanagedPointer, i, arr[i] ? (byte)1 : (byte)0);
            }

            return memory;
        }

        public static UnmanagedBlock ToArrayPtr32(this bool[] arr)
        {
            var memory = new UnmanagedBlock(sizeof(int) * arr.Length); // marshal bools are 4 bytes
            for (var i = 0; i < arr.Length; i++)
            {
                Marshal.WriteByte(memory.UnmanagedPointer, i * sizeof(int), arr[i] ? (byte)1 : (byte)0);
            }

            return memory;
        }

        // Only supports ASCII chars
        public static void StringValue(this RangeAccessor<byte> range, string newValue)
        {
            newValue = newValue.Substring(0, Math.Min(newValue.Length, range.Count));
            unsafe
            {
                new Span<byte>(Encoding.ASCII.GetBytes(newValue)).CopyTo(new Span<byte>(range.Data, range.Count));
            }
        }
        
        public static void StringValue(this RangeAccessor<short> range, string newValue)
        {
            newValue = newValue.Substring(0, Math.Min(newValue.Length, range.Count));
            unsafe
            {
                newValue.AsSpan().CopyTo(new Span<char>(range.Data, range.Count));
            }
        }
        
        public static void StringValue(this RangeAccessor<ushort> range, string newValue)
        {
            newValue = newValue.Substring(0, Math.Min(newValue.Length, range.Count));
            unsafe
            {
                newValue.AsSpan().CopyTo(new Span<char>(range.Data, range.Count));
            }
        }

        public static ColorList GetColorList(this ImGuiStylePtr stylePtr)
        {
            return new ColorList(stylePtr.Colors);
        }

        public readonly struct ColorList
        {
            private readonly RangeAccessor<Vector4> _range;

            public ColorListColor this[ImGuiCol color]
            {
                get => _range[(int) color];
                set => _range[(int) color] = value;
            }

            public ColorList(RangeAccessor<Vector4> range)
            {
                _range = range;
            }

            public readonly struct ColorListColor
            {
                private const float MaxByteColor = 256F;

                public readonly float R;
                public readonly float G;
                public readonly float B;
                public readonly float A;
                
                public ColorListColor(Vector4 color)
                {
                    R = color.X;
                    G = color.Y;
                    B = color.Z;
                    A = color.W;
                }

                public ColorListColor(Vector3 color)
                {
                    R = color.X;
                    G = color.Y;
                    B = color.Z;
                    A = 1F;
                }

                public ColorListColor(Color color)
                {
                    R = color.R / MaxByteColor;
                    G = color.G / MaxByteColor;
                    B = color.B / MaxByteColor;
                    A = color.A / MaxByteColor;
                }

                public ColorListColor(uint rgba)
                {
                    R = (rgba >> 24 & 0xFF) / MaxByteColor;
                    G = (rgba >> 16 & 0xFF) / MaxByteColor;
                    B = (rgba >> 8 & 0xFF) / MaxByteColor;
                    A = (rgba & 0xFF) / MaxByteColor;
                }

                public ColorListColor(byte r, byte g, byte b, byte a = byte.MaxValue)
                {
                    R = r / MaxByteColor;
                    G = g / MaxByteColor;
                    B = b / MaxByteColor;
                    A = a / MaxByteColor;
                }

                public static implicit operator Vector4(ColorListColor color) => new Vector4(color.R, color.G, color.B, color.A);
                public static implicit operator ColorListColor(Vector4 color) => new ColorListColor(color);
                public static implicit operator ColorListColor(Vector3 color) => new ColorListColor(color);
                public static implicit operator ColorListColor(Color color) => new ColorListColor(color);
                public static implicit operator ColorListColor(uint rgba) => new ColorListColor(rgba);
            }
        }
    }
}