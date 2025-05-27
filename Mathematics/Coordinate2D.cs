/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Coordinate2D.cs
 * PURPOSE:     A more clever way to handle some 2D coordinate Stuff
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable BadBracesSpaces
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mathematics
{
    /// <inheritdoc />
    /// <summary>
    ///     Coordinate 2d Helper Class
    /// </summary>
    [DebuggerDisplay("X = {X}, Y = {Y}, Id = {Id}")]
    public sealed class Coordinate2D : ICloneable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Coordinate2D" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Coordinate2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coordinate2D" /> class.
        /// </summary>
        /// <param name="x">The x in double.</param>
        /// <param name="y">The yin double.</param>
        public Coordinate2D(double x, double y)
        {
            X = (int)Math.Round(x, 1, MidpointRounding.AwayFromZero);
            Y = (int)Math.Round(y, 1, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coordinate2D" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        public Coordinate2D(int x, int y, int width)
        {
            X = x;
            Y = y;
            Id = CalculateId(x, y, width);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coordinate2D" /> class.
        /// </summary>
        public Coordinate2D()
        {
        }

        /// <summary>
        ///     Gets the null point.
        /// </summary>
        /// <value>
        ///     The null point.
        /// </value>
        public static Coordinate2D NullPoint { get; } = new(0, 0);

        /// <summary>
        ///     Gets the identifier of the Coordinate in the 2D System.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int Id { get; }

        /// <summary>
        ///     Gets or sets the y.
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public int Y { get; set; }

        /// <summary>
        ///     Gets or sets the x.
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public int X { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            // Create a new instance of Coordinate2D and copy the properties
            return new Coordinate2D(X, Y);
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="width">The width.</param>
        /// <returns>Instance of Coordinate 2D with the help of the Id</returns>
        public static Coordinate2D GetInstance(int id, int width)
        {
            return new Coordinate2D { X = IdToX(id, width), Y = IdToY(id, width) };
        }

        /// <summary>
        ///     Equals the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Equal or not</returns>
        public bool Equals(Coordinate2D other)
        {
            return X.Equals(other?.X) && Y.Equals(other?.Y);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Coordinate2D other && Equals(other);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Coordinate2D first, Coordinate2D second)
        {
            return second is not null && first is not null && first.X == second.X && first.Y == second.Y;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Coordinate2D first, Coordinate2D second)
        {
            return second is not null && first is not null && (first.X != second.X || first.Y != second.Y);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        /// <summary>
        ///     Calculates the unique identifier (ID) for a coordinate in a 2D grid, given the X and Y coordinates and the grid
        ///     width.
        ///     The method uses a row-major order (X is incremented first, then Y) to generate a linear ID for the coordinate.
        /// </summary>
        /// <param name="width">
        ///     The width of the 2D grid, i.e., the number of columns. This is used to calculate the linear ID
        ///     based on the X and Y coordinates.
        /// </param>
        /// <returns>
        ///     The unique identifier (ID) corresponding to the given X and Y coordinates in a 2D grid.
        ///     The ID is calculated using the formula: ID = (Y * width) + X.
        /// </returns>
        /// <remarks>
        ///     This method assumes that the grid is indexed in row-major order, where each row contains a number of elements equal
        ///     to the grid's width.
        ///     The ID starts from 0 for the top-left corner (X = 0, Y = 0) and increases as you move rightward through the rows,
        ///     then down to the next row.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CalculateId(int width)
        {
            return CalculateId(X, Y, width);
        }

        /// <summary>
        ///     Calculates the identifier.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <returns>The id of the coordinate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateId(int x, int y, int width)
        {
            return y * width + x;
        }

        /// <summary>
        ///     Converts a given identifier (ID) in a 2D grid to its corresponding X-coordinate.
        ///     The ID is treated as a linear index into a 2D grid, and this method computes the
        ///     X-coordinate based on that index and the grid's width.
        /// </summary>
        /// <param name="masterId">The identifier (linear index) of the coordinate in the grid.</param>
        /// <param name="width">The width of the 2D grid (the number of columns).</param>
        /// <returns>
        ///     The X-coordinate corresponding to the given identifier.
        /// </returns>
        /// <remarks>
        ///     This method uses the modulus operator to "wrap" the identifier into a valid X-coordinate.
        ///     It assumes that the IDs are assigned in a row-major order, starting at the top-left corner
        ///     and progressing left to right, top to bottom across the grid.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IdToX(int masterId, int width)
        {
            return masterId % width;
        }

        /// <summary>
        ///     Converts a given identifier (ID) in a 2D grid to its corresponding Y-coordinate.
        ///     The ID is treated as a linear index into a 2D grid, and this method computes the
        ///     Y-coordinate based on that index and the grid's width.
        /// </summary>
        /// <param name="masterId">The identifier (linear index) of the coordinate in the grid.</param>
        /// <param name="width">The width of the 2D grid (the number of columns).</param>
        /// <returns>
        ///     The Y-coordinate corresponding to the given identifier.
        /// </returns>
        /// <remarks>
        ///     This method uses integer division to calculate the row (Y-coordinate) of the identifier.
        ///     It assumes that the IDs are assigned in a row-major order, where each new row increments the Y-coordinate.
        ///     For example, if width = 5, the ID 6 would map to the second row (Y = 1), and ID 12 would map to the third row (Y =
        ///     2).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IdToY(int masterId, int width)
        {
            return masterId / width;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector3D" /> to <see cref="Coordinate2D" />.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Coordinate2D(Vector3D first)
        {
            return new Coordinate2D(first.RoundedX, first.RoundedY);
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(MathResources.StrX, X, MathResources.StrY, Y, MathResources.StrId, Id);
        }
    }
}