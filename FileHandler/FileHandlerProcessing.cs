/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     FileHandler
* FILE:        FileHandler/FileHandlerProcessing.cs
* PURPOSE:     Helper Methods for the FileHandler library
* PROGRAMMER:  Peter Geinitz (Wayfarer)
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace FileHandler
{
    /// <summary>
    ///     The file handler processing class.
    /// </summary>
    internal static class FileHandlerProcessing
    {
        /// <summary>
        ///     Cleans up the file extension list by removing dots.
        /// </summary>
        /// <param name="fileExtList">The file extension list.</param>
        /// <returns>The cleaned up list of file extensions.</returns>
        internal static List<string> CleanUpExtensionList(IEnumerable<string> fileExtList)
        {
            if (fileExtList == null)
            {
                throw new ArgumentNullException(nameof(fileExtList), FileHandlerResources.ErrorFileExtension);
            }

            return fileExtList.Select(ext => ext.Replace(FileHandlerResources.Dot, string.Empty)).ToList();
        }

        /// <summary>
        ///     Gets the subfolder path relative to the root directory and combines it with the target directory.
        /// </summary>
        /// <param name="element">The path of the element.</param>
        /// <param name="root">The root directory path.</param>
        /// <param name="target">The target directory path.</param>
        /// <returns>The combined target folder path.</returns>
        /// <exception cref="ArgumentException">Thrown when any of the input paths are invalid.</exception>
        internal static string GetSubFolder(string element, string root, string target)
        {
            var elementDir = Path.GetFullPath(element);
            var rootDir = Path.GetFullPath(root);

            if (!elementDir.StartsWith(rootDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(FileHandlerResources.ErrorInvalidPath);
            }

            var relativePath = elementDir.Substring(rootDir.Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return Path.Combine(target, relativePath);
        }

        /// <summary>
        ///     Collects all files with a specific extension from the target folder.
        /// </summary>
        /// <param name="path">The target folder path.</param>
        /// <param name="appendix">The file extension.</param>
        /// <param name="subdirectories">Indicates whether to include subdirectories.</param>
        /// <returns>List of files with the specified extension.</returns>
        /// <exception cref="FileHandlerException">Thrown when the path is empty or null.</exception>
        [return: MaybeNull]
        internal static List<string> GetFilesByExtension(string path, string appendix, bool subdirectories)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);
            }

            if (!Directory.Exists(path))
            {
                return null;
            }

            if (string.IsNullOrEmpty(appendix))
            {
                appendix = FileHandlerResources.Star;
            }

            appendix = appendix.Replace(FileHandlerResources.Dot, string.Empty);

            var option = subdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return Directory.EnumerateFiles(path, $"{FileHandlerResources.StarDot}{appendix}", option).ToList();
        }

        /// <summary>
        ///     Search the root Path.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The root<see cref="string" />.</returns>
        internal static string SearchRoot(IReadOnlyCollection<string> source)
        {
            return source.OrderBy(path => path.Length).First();
        }

        /// <summary>
        ///     Validates the paths.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal static void ValidatePaths(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);
            }

            if (source.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEqualPath);
            }
        }
    }
}
