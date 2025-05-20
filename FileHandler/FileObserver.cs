/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileObserver.cs
 * PURPOSE:     File Watcher, that observes changes to Folder.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global

using System;
using System.IO;
using System.Threading.Tasks;

namespace FileHandler
{
    public class FileObserver
    {
        /// <summary>
        ///     Searches the folder asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        public async Task SearchFolderAsync(string path)
        {
            var watcher = new FileSystemWatcher(path)
            {
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName,
                IncludeSubdirectories = true,
                InternalBufferSize = 64 * 1024 // Increase buffer size to handle more events
            };

            watcher.Created += OnCreated;
            watcher.Changed += OnChanged;
            watcher.Deleted += OnDeleted;
            watcher.Error += OnError;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Monitoring folder for changes. Press any key to stop.");
            await Task.Run(Console.ReadKey); // Run in background to avoid blocking

            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        /// <summary>
        ///     Called when [created].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File or folder '{e.FullPath}' was created.");
        }

        /// <summary>
        ///     Called when [changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File or folder '{e.FullPath}' was modified.");
        }

        /// <summary>
        ///     Called when [deleted].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File or folder '{e.FullPath}' was deleted.");
        }

        /// <summary>
        ///     Called when [error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ErrorEventArgs" /> instance containing the event data.</param>
        private static void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Error occurred: " + e.GetException().Message);
        }
    }
}
