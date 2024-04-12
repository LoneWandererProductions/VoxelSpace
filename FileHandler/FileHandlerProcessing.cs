/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandlerProcessing.cs
 * PURPOSE:     Helper Methods for the FileHandler library
 * PROGRAMER:   Peter Geinitz (Wayfarer)
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
        ///     Clean the up extension list.
        /// </summary>
        /// <param name="fileExtList">The fileExtList.</param>
        /// <returns>The <see cref="T:List{string}" />.</returns>
        internal static List<string> CleanUpExtensionList(IEnumerable<string> fileExtList)
        {
            return fileExtList.Select(appendix => appendix.Replace(FileHandlerResources.Dot, string.Empty)).ToList();
        }

        /// <summary>
        ///     Get the sub folder.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="root">The root.</param>
        /// <param name="target">The target.</param>
        /// <returns>The target Folder<see cref="string" />.</returns>
        internal static string GetSubFolder(string element, string root, string target)
        {
            element = Path.GetDirectoryName(element);
            root = Path.GetDirectoryName(root);

            // ReSharper disable once PossibleNullReferenceException, already checked
            var len = Math.Min(root.Length,
                // ReSharper disable once PossibleNullReferenceException, already checked
                element.Length);

            var index = 0;
            while (index < len)
            {
                if (root[index] != element[index])
                {
                    break;
                }

                index++;
            }

            element = element.Remove(0, index);

            if (element.Length == 0)
            {
                return target;
            }

            //Needed to remove the trailing \
            element = element.Remove(0, 1);
            return Path.Combine(target, element);
        }

        /// <summary>
        ///     Collects all files with a specific Extension
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="appendix">File Extension</param>
        /// <param name="subdirectories">Include Sub-folders</param>
        /// <returns>List of Files, with extension</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
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

            //cleanups just in Case
            appendix = appendix.Replace(FileHandlerResources.Dot, string.Empty);

            var option = subdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return Directory.EnumerateFiles(path, string.Concat(FileHandlerResources.StarDot, appendix), option)
                .ToList();
        }
    }
}
