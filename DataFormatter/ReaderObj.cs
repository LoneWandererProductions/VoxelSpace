/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/ReaderObj.cs
 * PURPOSE:     A really basic obj File reader, does the basic stuff nothing more!
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DataFormatter
{
    /// <summary>
    ///     Basic implementation to read Blender Files
    ///     Not feature complete
    /// </summary>
    public static class ReaderObj
    {
        /// <summary>
        ///     Reads the object.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Readable Obj File</returns>
        [return: MaybeNull]
        public static ObjFile ReadObj(string filePath)
        {
            var lst = ReadText.ReadFile(filePath);
            if (lst == null) return null;

            var vectors = new List<TertiaryVector>();
            var faces = new List<TertiaryFace>();
            var other = new List<string>();

            foreach (var trim in lst.Select(line => line.Trim())
                         .Where(trim => !trim.StartsWith(DataFormatterResources.Comment)))
            {
                if (trim.StartsWith(DataFormatterResources.Vector))
                {
                    var cache = trim.Remove(0, 2);
                    //cache = cache.Replace('.', ',');

                    var bits = DataHelper.GetParts(cache, DataFormatterResources.Space);

                    var check = double.TryParse(bits[0], out var x);
                    if (!check) continue;

                    check = double.TryParse(bits[1], out var y);
                    if (!check) continue;

                    check = double.TryParse(bits[1], out var z);
                    if (!check) continue;

                    var vector = new TertiaryVector { X = x, Y = y, Z = z };

                    vectors.Add(vector);

                    continue;
                }

                if (trim.StartsWith(DataFormatterResources.Face))
                {
                    var cache = trim.Remove(0, 2);
                    var bits = DataHelper.GetParts(cache, DataFormatterResources.Space);

                    var check = int.TryParse(bits[0], out var x);
                    if (!check) continue;

                    check = int.TryParse(bits[1], out var y);
                    if (!check) continue;

                    check = int.TryParse(bits[1], out var z);
                    if (!check) continue;

                    var vector = new TertiaryFace { X = x, Y = y, Z = z };

                    faces.Add(vector);

                    continue;
                }

                other.Add(trim);
            }

            return new ObjFile { Face = faces, Vectors = vectors, Other = other };
        }
    }
}