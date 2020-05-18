using System;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;

namespace HSNXT.QuickAndDirtyGui.Extensions
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
    
    public static class Extensions
    {
        public static NativeStruct<T> ToPtrClr<T>(this T structure, bool deleteOld = false) where T : unmanaged
            => NativeStruct<T>.Get(structure, deleteOld);

        public static NativeStruct<T> ToPtrMarshal<T>(this T structure, bool deleteOld = false) where T : unmanaged
            => NativeStruct<T>.GetMarshaled(structure, deleteOld);

        public static NativeArray ToArrayPtrClr<T>(this T[] arr) where T : unmanaged
            => NativeArray.From(arr);

        public static NativeArray ToArrayPtrAnsi(this char[] arr)
        {
            var nativeArr = new NativeArray(arr.Length);
            Encoding.ASCII.GetBytes(arr.AsSpan(), nativeArr.Span);
            nativeArr.Set(arr);
            return nativeArr;
        }

        public static NativeArray ToArrayPtrWide(this char[] arr)
        {
            // .NET chars are already 16-bit by default, no conversion is necessary
            return arr.ToArrayPtrClr();
        }

        public static NativeArray ToArrayPtr8(this bool[] arr)
        {
            // .NET booleans are already 8-bit wide by default, no conversion is necessary
            return arr.ToArrayPtrClr();
        }

        public static NativeArray ToArrayPtr32(this bool[] arr)
        {
            var memory = new NativeArray(sizeof(int) * arr.Length); // marshal bools are 4 bytes
            var span = memory.As<int>();
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = arr[i] ? 1 : 0;
            }
            return memory;
        }

        // Only supports ASCII chars
        // TODO null out the rest of the range
        public static unsafe void StringValue(this RangeAccessor<byte> range, string newValue)
        {
            newValue = newValue.Substring(0, Math.Min(newValue.Length, range.Count));
            Encoding.ASCII.GetBytes(newValue).AsSpan().CopyTo(new Span<byte>(range.Data, range.Count));
        }

        public static unsafe void StringValue(this RangeAccessor<short> range, string newValue)
        {
            newValue = newValue.Substring(0, Math.Min(newValue.Length, range.Count));
            newValue.AsSpan().CopyTo(new Span<char>(range.Data, range.Count));
        }

        public static unsafe void StringValue(this RangeAccessor<ushort> range, string newValue)
        {
            newValue = newValue.Substring(0, Math.Min(newValue.Length, range.Count));
            newValue.AsSpan().CopyTo(new Span<char>(range.Data, range.Count));
        }

        public static ImguiColors GetColors(this ImGuiStylePtr stylePtr)
        {
            return new ImguiColors(stylePtr.Colors);
        }
    }
}