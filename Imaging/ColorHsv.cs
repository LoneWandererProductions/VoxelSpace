/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ColorHsv.cs
 * PURPOSE:     General Conversions of all Types of Color Displays, Todo Sort out Degree and radian a bit more
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCE:      https://manufacture.tistory.com/33
 *              https://www.rapidtables.com/convert/color/rgb-to-hsv.html
 *              https://en.wikipedia.org/wiki/HSL_and_HSV
 *              https://docs.microsoft.com/de-de/dotnet/fundamentals/code-analysis/quality-rules/ca1036
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
// ReSharper disable NonReadonlyMemberInGetHashCode

using System;
using System.Diagnostics;
using System.Windows.Media;
using ExtendedSystemObjects;
using Mathematics;

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     HSV to RGP
    ///     And other Conversions
    /// </summary>
    public sealed class ColorHsv : IEquatable<ColorHsv>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorHsv" /> class.
        /// </summary>
        /// <param name="h">The h. Hue.</param>
        /// <param name="s">The s. Saturation</param>
        /// <param name="v">The v. Value</param>
        /// <param name="a">The Hue value.</param>
        public ColorHsv(double h, double s, double v, int a)
        {
            RgbFromHsv(h, s, v, a);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorHsv" /> class.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="a">The Hue value.</param>
        public ColorHsv(int r, int g, int b, int a)
        {
            if (r == -1)
            {
                return;
            }

            if (g == -1)
            {
                return;
            }

            if (b == -1)
            {
                return;
            }

            ColorToHsv(r, g, b, a);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorHsv" /> class.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <param name="a">The Hue value.</param>
        public ColorHsv(string hex, int a)
        {
            RbgHex(hex);
            A = a;
        }

        /// <summary>
        ///     The Hue Value, in our Case x.
        /// </summary>
        public double H { get; set; }

        /// <summary>
        ///     The s Value, saturation.
        /// </summary>
        public double S { get; set; }

        /// <summary>
        ///     The v, value
        /// </summary>
        public double V { get; set; }

        /// <summary>
        ///     The Hue Value, in our Case x.
        /// </summary>
        public int R { get; private set; }

        /// <summary>
        ///     The s Value, saturation.
        /// </summary>
        public int G { get; private set; }

        /// <summary>
        ///     The v, value
        /// </summary>
        public int B { get; private set; }

        /// <summary>
        ///     The A, value, Alpha Channel
        /// </summary>
        public int A { get; private set; }

        /// <summary>
        ///     Hex Value of Color
        /// </summary>
        public string Hex { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     HSV, A Values are enough to check equality
        /// </summary>
        /// <param name="other">ColorHsv Object</param>
        /// <returns>If equal or not</returns>
        public bool Equals(ColorHsv other)
        {
            if (other == null)
            {
                return false;
            }

            if (R != other.R)
            {
                return false;
            }

            if (G != other.G)
            {
                return false;
            }

            if (B != other.B)
            {
                return false;
            }

            return A == other.A;
        }

        /// <summary>
        ///     RGBs from HSV.
        ///     https://stackoverflow.com/questions/1335426/is-there-a-built-in-c-net-system-api-for-hsv-to-rgb
        /// </summary>
        /// <param name="h">The h. Degree, max 360 degree</param>
        /// <param name="s">The s. in our case x.</param>
        /// <param name="v">The v.</param>
        /// <param name="a">The A. Alpha Channel.</param>
        public void RgbFromHsv(double h, double s, double v, int a)
        {
            H = h;
            S = s;
            V = v;
            A = a;

            var degree = h * 180 / Math.PI;

            if (degree is > 360 or < 0 || s is > 1 or < 0 || v is > 1 or < 0)
            {
                return;
            }

            var c = v * s;
            var x = c * (1 - Math.Abs((degree / 60 % 2) - 1));
            var m = v - c;

            double r = 0, g = 0, b = 0;

            if (degree is < 60 and > 0)
            {
                r = c;
                g = x;
            }

            if (degree is < 120 and > 60)
            {
                r = x;
                g = c;
            }

            if (degree is < 180 and > 120)
            {
                g = c;
                b = x;
            }

            if (degree is < 240 and > 180)
            {
                g = x;
                b = c;
            }

            if (degree is < 300 and > 240)
            {
                r = x;
                b = c;
            }

            if (degree is < 360 and > 300)
            {
                r = c;
                b = x;
            }

            R = (int)((r + m) * 255);
            G = (int)((g + m) * 255);
            B = (int)((b + m) * 255);

            GetHex();
        }

        /// <summary>
        ///     RBGs the hexadecimal.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        public void RbgHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return;
            }

            Color color;

            try
            {
                color = (Color)ColorConverter.ConvertFromString(hex);
            }
            catch (NullReferenceException e)
            {
                Trace.WriteLine(e);
                return;
            }
            catch (FormatException e)
            {
                Trace.WriteLine(e);
                return;
            }

            Hex = hex;
            ColorToHsv(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        ///     Colors to HSV.
        /// </summary>
        /// <param name="r">The r. Red.</param>
        /// <param name="g">The g. Green.</param>
        /// <param name="b">The b. Blue.</param>
        /// <param name="a">The A. Alpha Channel.</param>
        public void ColorToHsv(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            H = GetHue(r, g, b);
            S = max == 0 ? 0 : 1d - (1d * min / max);
            V = max / 255d;

            GetHex();
        }

        /// <summary>
        ///     Hexadecimals to color.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        public void HexToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return;
            }

            var color = (Color)ColorConverter.ConvertFromString(hex);

            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;

            ColorToHsv(R, G, B, A);
        }

        /// <summary>
        ///     Gets the color, Media Name Space
        /// </summary>
        /// <returns>Color Value</returns>
        public Color GetColor()
        {
            return Color.FromArgb((byte)R, (byte)G, (byte)B, (byte)A);
        }

        /// <summary>
        ///     Override Equals
        /// </summary>
        /// <param name="obj">ColorHsv Object</param>
        /// <returns>If equal or not</returns>
        public override bool Equals(object obj)
        {
            return obj is ColorHsv other && Equals(other);
        }

        /// <summary>
        ///     Generates hash for our object
        ///     Generated from Hex and Alpha Channel
        /// </summary>
        /// <returns>The Hash Code</returns>
        public override int GetHashCode()
        {
            return Convert.ToInt32($"{Hex}{A}");
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(ColorHsv left, ColorHsv right)
        {
            return left?.Equals(right) == true;
        }

        /// <summary>
        ///     Overwrite not equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>Check if it is not equal</returns>
        public static bool operator !=(ColorHsv left, ColorHsv right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Overwrite smaller as
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>Check if smaller</returns>
        public static bool operator <(ColorHsv left, ColorHsv right)
        {
            return left.H < right.H;
        }

        /// <summary>
        ///     Overwrite bigger as
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>Check if bigger</returns>
        public static bool operator >(ColorHsv left, ColorHsv right)
        {
            return left.H > right.H;
        }

        /// <summary>
        ///     Gets the hue.
        /// </summary>
        /// <param name="r">The r. Red.</param>
        /// <param name="g">The g. Green.</param>
        /// <param name="b">The b. Blue.</param>
        /// <returns>The hue Value</returns>
        private static double GetHue(int r, int g, int b)
        {
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);

            if (min.IsEqualTo(max, 10))
            {
                return 0;
            }

            double hue;

            if (max.IsEqualTo(r, 10))
            {
                hue = (g - b) / (max - min);
            }
            else if (max.IsEqualTo(g, 10))
            {
                hue = 2f + ((b - r) / (max - min));
            }
            else
            {
                hue = 4f + ((r - g) / (max - min));
            }

            hue *= 60;

            if (hue < 0)
            {
                hue += 360;
            }

            return hue * Math.PI / 180;
        }

        /// <summary>
        ///     Gets the hexadecimal.
        /// </summary>
        private void GetHex()
        {
            Hex = string.Concat("#", $"{R:X2}{G:X2}{B:X2}");
        }
    }
}
