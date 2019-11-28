using System;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using ImGuiNET;
using MI = System.Runtime.CompilerServices.MethodImplAttribute;

namespace HSNXT.QuickAndDirtyGui
{
    public static unsafe class ImportMeHelpers
    {
        private const MethodImplOptions I = MethodImplOptions.AggressiveInlining;

        /*
         * Create unsafe instances, deleting these is your problem!
         */
        [MI(I)] public static ImColor* new_ImColor() => ImGuiNative.ImColor_ImColor();
        [MI(I)] public static ImDrawCmd* new_ImDrawCmd() => ImGuiNative.ImDrawCmd_ImDrawCmd();
        [MI(I)] public static ImDrawData* new_ImDrawData() => ImGuiNative.ImDrawData_ImDrawData();
        [MI(I)] public static ImDrawList* new_ImDrawList(IntPtr sharedData) => ImGuiNative.ImDrawList_ImDrawList(sharedData);
        [MI(I)] public static ImDrawListSplitter* new_ImDrawListSplitter() => ImGuiNative.ImDrawListSplitter_ImDrawListSplitter();
        [MI(I)] public static ImFont* new_ImFont() => ImGuiNative.ImFont_ImFont();
        [MI(I)] public static ImFontAtlas* new_ImFontAtlas() => ImGuiNative.ImFontAtlas_ImFontAtlas();
        [MI(I)] public static ImFontAtlasCustomRect* new_ImFontAtlasCustomRect() => ImGuiNative.ImFontAtlasCustomRect_ImFontAtlasCustomRect();
        [MI(I)] public static ImFontConfig* new_ImFontConfig() => ImGuiNative.ImFontConfig_ImFontConfig();
        [MI(I)] public static ImFontGlyphRangesBuilder* new_ImFontGlyphRangesBuilder() => ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder();
        [MI(I)] public static ImGuiInputTextCallbackData* new_ImGuiInputTextCallbackData() => ImGuiNative.ImGuiInputTextCallbackData_ImGuiInputTextCallbackData();
        [MI(I)] public static ImGuiIO* new_ImGuiIO() => ImGuiNative.ImGuiIO_ImGuiIO();
        [MI(I)] public static ImGuiListClipper* new_ImGuiListClipper(int itemsCount, float itemsHeight) => ImGuiNative.ImGuiListClipper_ImGuiListClipper(itemsCount, itemsHeight);
        [MI(I)] public static ImGuiOnceUponAFrame* new_ImGuiOnceUponAFrame() => ImGuiNative.ImGuiOnceUponAFrame_ImGuiOnceUponAFrame();
        [MI(I)] public static ImGuiPayload* new_ImGuiPayload() => ImGuiNative.ImGuiPayload_ImGuiPayload();
        [MI(I)] public static ImGuiStyle* new_ImGuiStyle() => ImGuiNative.ImGuiStyle_ImGuiStyle();
        [MI(I)] public static ImGuiTextBuffer* new_ImGuiTextBuffer() => ImGuiNative.ImGuiTextBuffer_ImGuiTextBuffer();
        [MI(I)] public static ImGuiTextFilter* new_ImGuiTextFilter(byte* defaultFilter) => ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(defaultFilter);
        [MI(I)] public static ImGuiTextRange* new_ImGuiTextRange() => ImGuiNative.ImGuiTextRange_ImGuiTextRange();

        /*
         * Create instances that get deleted like they should be as long as you call Dispose
         */
         [MI(I)] public static UnmanagedPointer<ImColorPtr> new_ImColorPtr() => new UnmanagedPointer<ImColorPtr>(ImGuiNative.ImColor_ImColor());
         [MI(I)] public static UnmanagedPointer<ImDrawCmdPtr> new_ImDrawCmdPtr() => new UnmanagedPointer<ImDrawCmdPtr>(ImGuiNative.ImDrawCmd_ImDrawCmd());
         [MI(I)] public static UnmanagedPointer<ImDrawDataPtr> new_ImDrawDataPtr() => new UnmanagedPointer<ImDrawDataPtr>(ImGuiNative.ImDrawData_ImDrawData());
         [MI(I)] public static UnmanagedPointer<ImDrawListPtr> new_ImDrawListPtr(IntPtr sharedData) => new UnmanagedPointer<ImDrawListPtr>(ImGuiNative.ImDrawList_ImDrawList(sharedData));
         [MI(I)] public static UnmanagedPointer<ImDrawListSplitterPtr> new_ImDrawListSplitterPtr() => new UnmanagedPointer<ImDrawListSplitterPtr>(ImGuiNative.ImDrawListSplitter_ImDrawListSplitter());
         [MI(I)] public static UnmanagedPointer<ImFontPtr> new_ImFontPtr() => new UnmanagedPointer<ImFontPtr>(ImGuiNative.ImFont_ImFont());
         [MI(I)] public static UnmanagedPointer<ImFontAtlasPtr> new_ImFontAtlasPtr() => new UnmanagedPointer<ImFontAtlasPtr>(ImGuiNative.ImFontAtlas_ImFontAtlas());
         [MI(I)] public static UnmanagedPointer<ImFontAtlasCustomRectPtr> new_ImFontAtlasCustomRectPtr() => new UnmanagedPointer<ImFontAtlasCustomRectPtr>(ImGuiNative.ImFontAtlasCustomRect_ImFontAtlasCustomRect());
         [MI(I)] public static UnmanagedPointer<ImFontConfigPtr> new_ImFontConfigPtr() => new UnmanagedPointer<ImFontConfigPtr>(ImGuiNative.ImFontConfig_ImFontConfig());
         [MI(I)] public static UnmanagedPointer<ImFontGlyphRangesBuilderPtr> new_ImFontGlyphRangesBuilderPtr() => new UnmanagedPointer<ImFontGlyphRangesBuilderPtr>(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
         [MI(I)] public static UnmanagedPointer<ImGuiInputTextCallbackDataPtr> new_ImGuiInputTextCallbackDataPtr() => new UnmanagedPointer<ImGuiInputTextCallbackDataPtr>(ImGuiNative.ImGuiInputTextCallbackData_ImGuiInputTextCallbackData());
         [MI(I)] public static UnmanagedPointer<ImGuiIOPtr> new_ImGuiIOPtr() => new UnmanagedPointer<ImGuiIOPtr>(ImGuiNative.ImGuiIO_ImGuiIO());
         [MI(I)] public static UnmanagedPointer<ImGuiListClipperPtr> new_ImGuiListClipperPtr(int itemsCount, float itemsHeight) => new UnmanagedPointer<ImGuiListClipperPtr>(ImGuiNative.ImGuiListClipper_ImGuiListClipper(itemsCount, itemsHeight));
         [MI(I)] public static UnmanagedPointer<ImGuiOnceUponAFramePtr> new_ImGuiOnceUponAFramePtr() => new UnmanagedPointer<ImGuiOnceUponAFramePtr>(ImGuiNative.ImGuiOnceUponAFrame_ImGuiOnceUponAFrame());
         [MI(I)] public static UnmanagedPointer<ImGuiPayloadPtr> new_ImGuiPayloadPtr() => new UnmanagedPointer<ImGuiPayloadPtr>(ImGuiNative.ImGuiPayload_ImGuiPayload());
         [MI(I)] public static UnmanagedPointer<ImGuiStylePtr> new_ImGuiStylePtr() => new UnmanagedPointer<ImGuiStylePtr>(ImGuiNative.ImGuiStyle_ImGuiStyle());
         [MI(I)] public static UnmanagedPointer<ImGuiTextBufferPtr> new_ImGuiTextBufferPtr() => new UnmanagedPointer<ImGuiTextBufferPtr>(ImGuiNative.ImGuiTextBuffer_ImGuiTextBuffer());
         [MI(I)] public static UnmanagedPointer<ImGuiTextFilterPtr> new_ImGuiTextFilterPtr(byte* defaultFilter) => new UnmanagedPointer<ImGuiTextFilterPtr>(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(defaultFilter));
         [MI(I)] public static UnmanagedPointer<ImGuiTextRangePtr> new_ImGuiTextRangePtr() => new UnmanagedPointer<ImGuiTextRangePtr>(ImGuiNative.ImGuiTextRange_ImGuiTextRange());
        
        internal static readonly ConstructorInfo IntPtrCtor
            = typeof(IntPtr).GetConstructor(new[] { typeof(void*) })
              ?? throw new InvalidOperationException($"IntPtr changed, this should never happen");
    }

    public readonly struct UnmanagedPointer<T> : IDisposable where T : unmanaged
    {
        private static readonly Action<T> Destroy;
        private static readonly Func<T, IntPtr> GetNativePtr;
        public T V { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        private static readonly MethodInfo ToVoidPtrMethod
            = typeof(UnmanagedPointer<T>).GetMethod(nameof(ToVoidPtr), BindingFlags.Static | BindingFlags.NonPublic);
        private static unsafe void* ToVoidPtr<T1>(T1* ptr) where T1 : unmanaged => ptr;

        static UnmanagedPointer()
        {
            var param = Expression.Parameter(typeof(T), "this");
            var method =
                typeof(T).GetMethod("Destroy")
                ?? throw new InvalidOperationException($"UnmanagedHandle<{typeof(T)}> - {typeof(T)} must have a Destroy method to be used");
            Destroy = Expression.Lambda<Action<T>>(Expression.Call(param, method), param).Compile();

            var propNativePtr =
                typeof(T).GetProperty("NativePtr")
                ?? throw new InvalidOperationException($"UnmanagedHandle<{typeof(T)}> - {typeof(T)} must have a NativePtr property to be used");
            var structType = propNativePtr.PropertyType.GetElementType();

            param = Expression.Parameter(typeof(T), "this");
            GetNativePtr = Expression.Lambda<Func<T, IntPtr>>(
                Expression.New(ImportMeHelpers.IntPtrCtor, 
                    Expression.Call(ToVoidPtrMethod.MakeGenericMethod(structType), Expression.Property(param, propNativePtr))
                ), param).Compile();
        }

        public UnmanagedPointer(T instance)
        {
            V = instance;
        }

        public static unsafe implicit operator void*(UnmanagedPointer<T> handle) => GetNativePtr(handle.V).ToPointer();
        public static implicit operator IntPtr(UnmanagedPointer<T> handle) => GetNativePtr(handle.V);
        public static implicit operator T(UnmanagedPointer<T> handle) => handle.V;

        public void Dispose() => Destroy(V);
    }
}