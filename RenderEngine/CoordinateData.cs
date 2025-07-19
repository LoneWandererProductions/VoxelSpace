/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        CoordinateData.cs
 * PURPOSE:     Pixel data structure for rendering and Color information.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using OpenTK.Mathematics;

namespace RenderEngine
{
    public struct CoordinateData
    {
        public int X;
        public int Y;
        public Vector3 Color;
    }
}
