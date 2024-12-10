/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/PolyTriangle.cs
 * PURPOSE:     Helper Object to handle the description of the 3d object. It also supports more than 3 Vectors, in case we want to go full polygon.s
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using DataFormatter;

namespace Mathematics
{
    /// <inheritdoc />
    /// <summary>
    ///     In the future will be retooled to polygons.
    /// </summary>
    public sealed class PolyTriangle : IEquatable<PolyTriangle>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PolyTriangle" /> class.
        /// </summary>
        /// <param name="array">The array.</param>
        public PolyTriangle(IReadOnlyList<Vector3D> array)
        {
            Vertices = new Vector3D[array.Count];

            for (var i = 0; i < array.Count; i++) Vertices[i] = array[i];
        }

        /// <summary>
        ///     Gets the normal.
        /// </summary>
        /// <value>
        ///     The normal.
        /// </value>
        public Vector3D Normal
        {
            get
            {
                var u = Vertices[1] - Vertices[0];
                var v = Vertices[2] - Vertices[0];

                return u.CrossProduct(v).Normalize();
            }
        }

        /// <summary>
        ///     Gets the vertex count.
        /// </summary>
        /// <value>
        ///     The vertex count.
        /// </value>
        public int VertexCount => Vertices.Length;

        /// <summary>
        ///     Gets or sets the vertices.
        /// </summary>
        /// <value>
        ///     The vertices.
        /// </value>
        public Vector3D[] Vertices { get; }

        /// <summary>
        ///     Gets or sets the <see cref="Vector3D" /> with the specified i.
        /// </summary>
        /// <value>
        ///     The <see cref="Vector3D" />.
        /// </value>
        /// <param name="i">The i.</param>
        /// <returns>vector by id</returns>
        public Vector3D this[int i]
        {
            get => Vertices[i];
            set => Vertices[i] = value;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public bool Equals(PolyTriangle other)
        {
            // If the other object is null, return false
            if (other == null) return false;

            // If the other object is the same instance as this object, return true
            if (ReferenceEquals(this, other)) return true;

            // If the number of vertices is different, the triangles are not equal
            if (VertexCount != other.VertexCount) return false;

            // Compare each vertex of the triangles
            for (var i = 0; i < VertexCount; i++)
                if (Vertices[i] != other.Vertices[i])
                    return false;

            // If all vertices are equal, the triangles are equal
            return true;
        }

        /// <summary>
        ///     Creates the triangle set.
        ///     Triangles need to be supplied on a CLOCKWISE order
        /// </summary>
        /// <param name="triangles">The triangles.</param>
        /// <returns>A list with Triangles, three Vectors in one Object</returns>
        public static List<PolyTriangle> CreateTri(List<TertiaryVector> triangles)
        {
            var polygons = new List<PolyTriangle>();

            for (var i = 0; i <= triangles.Count - 3; i += 3)
            {
                var v1 = triangles[i];
                var v2 = triangles[i + 1];
                var v3 = triangles[i + 2];

                var array = new[] { (Vector3D)v1, (Vector3D)v2, (Vector3D)v3 };
                var tri = new PolyTriangle(array);

                polygons.Add(tri);
            }

            return polygons;
        }

        /// <summary>
        ///     Gets the plot point.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>2d Vector, we only need these anyways for drawing.</returns>
        public Vector2D GetPlotPoint(int id)
        {
            return id > VertexCount || id < 0 ? null : (Vector2D)Vertices[id];
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Vertices[0].GetHashCode();
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = string.Empty;

            for (var i = 0; i < Vertices.Length; i++)
                str = string.Concat(str, i, MathResources.Separator, Vertices[i].ToString(), Environment.NewLine);

            return str;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is PolyTriangle other && Equals(other);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(PolyTriangle first, PolyTriangle second)
        {
            return second is not null && first is not null && first.Equals(second);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(PolyTriangle first, PolyTriangle second)
        {
            return second is not null && first is not null && first.Equals(second);
        }
    }
}