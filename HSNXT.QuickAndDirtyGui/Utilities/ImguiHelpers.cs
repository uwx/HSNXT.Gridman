using System;
using System.Runtime.CompilerServices;
using ImGuiNET;
using static System.Linq.Expressions.Expression;
using MI = System.Runtime.CompilerServices.MethodImplAttribute;

namespace HSNXT.QuickAndDirtyGui
{
    public static unsafe class ImguiStructHelpers
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
         [MI(I)] public static ImguiPointer<ImColorPtr> new_ImColorPtr() => new ImguiPointer<ImColorPtr>(ImGuiNative.ImColor_ImColor());
         [MI(I)] public static ImguiPointer<ImDrawCmdPtr> new_ImDrawCmdPtr() => new ImguiPointer<ImDrawCmdPtr>(ImGuiNative.ImDrawCmd_ImDrawCmd());
         [MI(I)] public static ImguiPointer<ImDrawDataPtr> new_ImDrawDataPtr() => new ImguiPointer<ImDrawDataPtr>(ImGuiNative.ImDrawData_ImDrawData());
         [MI(I)] public static ImguiPointer<ImDrawListPtr> new_ImDrawListPtr(IntPtr sharedData) => new ImguiPointer<ImDrawListPtr>(ImGuiNative.ImDrawList_ImDrawList(sharedData));
         [MI(I)] public static ImguiPointer<ImDrawListSplitterPtr> new_ImDrawListSplitterPtr() => new ImguiPointer<ImDrawListSplitterPtr>(ImGuiNative.ImDrawListSplitter_ImDrawListSplitter());
         [MI(I)] public static ImguiPointer<ImFontPtr> new_ImFontPtr() => new ImguiPointer<ImFontPtr>(ImGuiNative.ImFont_ImFont());
         [MI(I)] public static ImguiPointer<ImFontAtlasPtr> new_ImFontAtlasPtr() => new ImguiPointer<ImFontAtlasPtr>(ImGuiNative.ImFontAtlas_ImFontAtlas());
         [MI(I)] public static ImguiPointer<ImFontAtlasCustomRectPtr> new_ImFontAtlasCustomRectPtr() => new ImguiPointer<ImFontAtlasCustomRectPtr>(ImGuiNative.ImFontAtlasCustomRect_ImFontAtlasCustomRect());
         [MI(I)] public static ImguiPointer<ImFontConfigPtr> new_ImFontConfigPtr() => new ImguiPointer<ImFontConfigPtr>(ImGuiNative.ImFontConfig_ImFontConfig());
         [MI(I)] public static ImguiPointer<ImFontGlyphRangesBuilderPtr> new_ImFontGlyphRangesBuilderPtr() => new ImguiPointer<ImFontGlyphRangesBuilderPtr>(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
         [MI(I)] public static ImguiPointer<ImGuiInputTextCallbackDataPtr> new_ImGuiInputTextCallbackDataPtr() => new ImguiPointer<ImGuiInputTextCallbackDataPtr>(ImGuiNative.ImGuiInputTextCallbackData_ImGuiInputTextCallbackData());
         [MI(I)] public static ImguiPointer<ImGuiIOPtr> new_ImGuiIOPtr() => new ImguiPointer<ImGuiIOPtr>(ImGuiNative.ImGuiIO_ImGuiIO());
         [MI(I)] public static ImguiPointer<ImGuiListClipperPtr> new_ImGuiListClipperPtr(int itemsCount, float itemsHeight) => new ImguiPointer<ImGuiListClipperPtr>(ImGuiNative.ImGuiListClipper_ImGuiListClipper(itemsCount, itemsHeight));
         [MI(I)] public static ImguiPointer<ImGuiOnceUponAFramePtr> new_ImGuiOnceUponAFramePtr() => new ImguiPointer<ImGuiOnceUponAFramePtr>(ImGuiNative.ImGuiOnceUponAFrame_ImGuiOnceUponAFrame());
         [MI(I)] public static ImguiPointer<ImGuiPayloadPtr> new_ImGuiPayloadPtr() => new ImguiPointer<ImGuiPayloadPtr>(ImGuiNative.ImGuiPayload_ImGuiPayload());
         [MI(I)] public static ImguiPointer<ImGuiStylePtr> new_ImGuiStylePtr() => new ImguiPointer<ImGuiStylePtr>(ImGuiNative.ImGuiStyle_ImGuiStyle());
         [MI(I)] public static ImguiPointer<ImGuiTextBufferPtr> new_ImGuiTextBufferPtr() => new ImguiPointer<ImGuiTextBufferPtr>(ImGuiNative.ImGuiTextBuffer_ImGuiTextBuffer());
         [MI(I)] public static ImguiPointer<ImGuiTextFilterPtr> new_ImGuiTextFilterPtr(byte* defaultFilter) => new ImguiPointer<ImGuiTextFilterPtr>(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(defaultFilter));
         [MI(I)] public static ImguiPointer<ImGuiTextRangePtr> new_ImGuiTextRangePtr() => new ImguiPointer<ImGuiTextRangePtr>(ImGuiNative.ImGuiTextRange_ImGuiTextRange());
    }

    public readonly struct ImguiPointer<T> : IDisposable where T : unmanaged
    {
        private static readonly Action<T> Destroy;
        private static readonly Func<T, IntPtr> GetNativePtr;

        public T Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        static ImguiPointer()
        {
            if (typeof(T).Assembly != typeof(ImGui).Assembly)
            {
                throw new ArgumentException("Type not accepted, must be part of ImGui.NET", nameof(T));
            }

            var instanceParam = Parameter(typeof(T), "ptrInstance");

            var destroyMethod =
                typeof(T).GetMethod("Destroy")
                ?? throw new InvalidOperationException($"{typeof(T)} missing Destroy method");
            
            // ptrInstance => ptrInstance.Destroy()
            Destroy = Lambda<Action<T>>(instanceParam.Call(destroyMethod), instanceParam).Compile();

            var nativePtrProperty =
                typeof(T).GetProperty("NativePtr")
                ?? throw new InvalidOperationException($"{typeof(T)} missing NativePtr property");

            // ptrInstance => (IntPtr) ptrInstance.NativePtr
            GetNativePtr = Lambda<Func<T, IntPtr>>(instanceParam.Property(nativePtrProperty).Convert(typeof(IntPtr)), instanceParam).Compile();
        }

        public ImguiPointer(T instance)
        {
            Value = instance;
        }

        public static unsafe implicit operator void*(ImguiPointer<T> handle) => GetNativePtr(handle.Value).ToPointer();
        public static implicit operator IntPtr(ImguiPointer<T> handle) => GetNativePtr(handle.Value);
        public static implicit operator T(ImguiPointer<T> handle) => handle.Value;

        public void Dispose() => Destroy(Value);
    }
}