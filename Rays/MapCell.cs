/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        MapCell.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace Rays
{
    public class MapCell
    {
        public int WallId { get; set; }
        public int FloorId { get; set; }
        public int CeilingId { get; set; }
    }
}