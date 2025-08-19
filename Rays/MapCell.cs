/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        MapCell.cs
 * PURPOSE:     3D raycaster with per-cell primitives (cubes, ramps, multi-layered)
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;

namespace Rays
{
    public sealed class MapCell
    {
        public List<CellPrimitive> Primitives { get; } = new();
    }
}
