using System;
using System.Drawing;
using System.Numerics;

namespace HSNXT.QuickAndDirtyGui
{
    /// <summary>
    /// Single-precision floating point RGBA colors used by ImGui.
    /// </summary>
    public readonly struct Vector4Color
    {
        private const float MaxByteColor = 256F;

        public readonly float R;
        public readonly float G;
        public readonly float B;
        public readonly float A;
                
        public Vector4Color(Vector4 color)
        {
            R = color.X;
            G = color.Y;
            B = color.Z;
            A = color.W;
        }

        public Vector4Color(Vector3 color)
        {
            R = color.X;
            G = color.Y;
            B = color.Z;
            A = 1F;
        }

        public Vector4Color(Color color)
        {
            R = color.R / MaxByteColor;
            G = color.G / MaxByteColor;
            B = color.B / MaxByteColor;
            A = color.A / MaxByteColor;
        }

        public Vector4Color(uint rgba)
        {
            R = (rgba >> 24 & 0xFF) / MaxByteColor;
            G = (rgba >> 16 & 0xFF) / MaxByteColor;
            B = (rgba >> 8 & 0xFF) / MaxByteColor;
            A = (rgba & 0xFF) / MaxByteColor;
        }

        public Vector4Color(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            R = r / MaxByteColor;
            G = g / MaxByteColor;
            B = b / MaxByteColor;
            A = a / MaxByteColor;
        }

        public static implicit operator Vector4(Vector4Color color) => new Vector4(color.R, color.G, color.B, color.A);
        public static implicit operator Vector4Color(Vector4 color) => new Vector4Color(color);
        public static implicit operator Vector4Color(Vector3 color) => new Vector4Color(color);
        public static implicit operator Vector4Color(Color color) => new Vector4Color(color);
        public static implicit operator Vector4Color(uint rgba) => new Vector4Color(rgba);
    }
}