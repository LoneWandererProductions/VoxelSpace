/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/ObjFile.cs
 * PURPOSE:     Basic Object for Blender File
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * NOTE:        Blender Files don't start counting at 0! So add +1 offset.
 */

using System.Collections.Generic;

namespace DataFormatter
{
    /// <summary>
    ///     Object Presentation of an Object File for Vector Graphics, pretty much basic for now
    /// </summary>
    public sealed class ObjFile
    {
        /// <summary>
        ///     Gets the vectors.
        ///     Important! Blender Files don't start counting at 0! So add +1 offset.
        /// </summary>
        /// <value>
        ///     The vectors.
        /// </value>
        public List<TertiaryVector> Vectors { get; init; }

        /// <summary>
        ///     Gets the face to combine the Vectors
        /// </summary>
        /// <value>
        ///     The face.
        /// </value>
        public List<TertiaryFace> Face { get; init; }

        /// <summary>
        ///     Gets the other.
        /// </summary>
        /// <value>
        ///     The other.
        /// </value>
        public List<string> Other { get; init; }
    }
}