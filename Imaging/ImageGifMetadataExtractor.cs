/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     Imaging
* FILE:        Imaging/ImageGifMetadataExtractor.cs
* PURPOSE:     Get all the info of a gif
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

using System;
using System.IO;

namespace Imaging
{
    /// <summary>
    ///     Information about the gif
    /// </summary>
    internal static class ImageGifMetadataExtractor
    {
        /// <summary>
        ///     Extracts the GIF metadata.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">File not found.</exception>
        /// <exception cref="InvalidDataException">Not a valid GIF file.</exception>
        internal static ImageGifInfo ExtractGifMetadata(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(ImagingResources.FileNotFoundMessage, filePath);

            var metadata = new ImageGifInfo
            {
                Name = Path.GetFileName(filePath),
                Size = new FileInfo(filePath).Length
            };

            double lastFrameDelay = 0;

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);

            // Read GIF Header
            metadata.Header = new string(reader.ReadChars(ImagingResources.GifHeaderLength));
            if (!metadata.Header.StartsWith(ImagingResources.GifHeaderStart, StringComparison.Ordinal))
                throw new InvalidDataException(ImagingResources.InvalidGifMessage);

            // Logical Screen Descriptor
            metadata.Width = reader.ReadInt16();
            metadata.Height = reader.ReadInt16();
            var packedFields = reader.ReadByte();
            metadata.BackgroundColorIndex = reader.ReadByte();
            metadata.PixelAspectRatio = reader.ReadByte();

            metadata.HasGlobalColorTable = (packedFields & ImagingResources.GlobalColorTableFlag) != 0;
            metadata.ColorResolution = ((packedFields & ImagingResources.ColorResolutionMask) >> 4) + 1;
            metadata.GlobalColorTableSize = metadata.HasGlobalColorTable
                ? 3 * (1 << ((packedFields & ImagingResources.TableSizeMask) + 1))
                : 0;

            if (metadata.HasGlobalColorTable)
                reader.BaseStream.Seek(metadata.GlobalColorTableSize, SeekOrigin.Current);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var blockId = reader.ReadByte();
                if (blockId == ImagingResources.PaddingBlockId)
                {
                    Console.WriteLine(ImagingResources.SkipPaddingMessage);
                    continue;
                }

                Console.WriteLine(ImagingResources.ProcessingBlockMessage, blockId);

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    blockId = reader.ReadByte();
                    if (blockId == ImagingResources.PaddingBlockId)
                    {
                        Console.WriteLine(ImagingResources.SkipPaddingMessage);
                        continue;
                    }

                    Console.WriteLine(ImagingResources.ProcessingBlockMessage, blockId);

                    byte packed;
                    switch (blockId)
                    {
                        case ImagingResources.ExtensionIntroducer:

                            var extensionLabel = reader.ReadByte();

                            switch (extensionLabel)
                            {
                                case ImagingResources.ApplicationExtensionLabel:
                                    var blockSize = reader.ReadByte();
                                    var appIdentifier =
                                        new string(reader.ReadChars(ImagingResources.AppIdentifierLength));
                                    var appAuthCode = new string(reader.ReadChars(ImagingResources.AppAuthCodeLength));

                                    if (appIdentifier == ImagingResources.NetScapeIdentifier)
                                    {
                                        var subBlockSize = reader.ReadByte();
                                        var loopFlag = reader.ReadByte();
                                        metadata.LoopCount = reader.ReadInt16();
                                    }
                                    else
                                    {
                                        SkipExtensionBlocks(reader);
                                    }

                                    break;

                                case ImagingResources.GraphicsControlExtensionLabel:
                                    reader.BaseStream.Seek(1, SeekOrigin.Current);
                                    packed = reader.ReadByte();
                                    var delay = reader.ReadInt16();
                                    lastFrameDelay = delay / ImagingResources.DelayDivisor;
                                    reader.BaseStream.Seek(1, SeekOrigin.Current);
                                    break;

                                default:
                                    SkipExtensionBlocks(reader);
                                    break;
                            }

                            break;

                        case ImagingResources.ImageDescriptorId:
                            metadata.Frames.Add(new FrameInfo
                            {
                                Description = ImagingResources.ImageFrameDescription,
                                DelayTime = lastFrameDelay
                            });

                            reader.BaseStream.Seek(ImagingResources.ImageDescriptorLength, SeekOrigin.Current);
                            packed = reader.ReadByte();
                            if ((packed & ImagingResources.LocalColorTableFlag) != 0)
                            {
                                var tableSize = 3 * (1 << ((packed & ImagingResources.TableSizeMask) + 1));
                                reader.BaseStream.Seek(tableSize, SeekOrigin.Current);
                            }

                            while (true)
                            {
                                var subBlockSize = reader.ReadByte();
                                if (subBlockSize == ImagingResources.TerminatorBlockId) break;

                                reader.BaseStream.Seek(subBlockSize, SeekOrigin.Current);
                            }

                            break;

                        case ImagingResources.TrailerBlockId:
                            Console.WriteLine(ImagingResources.GifTrailerMessage);
                            return metadata;

                        default:
                            Console.WriteLine(ImagingResources.UnknownBlockMessage, blockId);
                            SkipUnknownBlock(reader, blockId);
                            break;
                    }
                }
            }

            return metadata;
        }

        /// <summary>
        ///     Skips the unknown block.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="blockId">The block identifier.</param>
        private static void SkipUnknownBlock(BinaryReader reader, byte blockId)
        {
            Console.WriteLine(ImagingResources.SkipUnknownBlockMessage, blockId);
            while (true)
            {
                var subBlockSize = reader.ReadByte();
                if (subBlockSize == ImagingResources.TerminatorBlockId) break;

                reader.BaseStream.Seek(subBlockSize, SeekOrigin.Current);
            }
        }

        /// <summary>
        ///     Skips the extension blocks.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private static void SkipExtensionBlocks(BinaryReader reader)
        {
            while (true)
            {
                var blockSize = reader.ReadByte();
                if (blockSize == ImagingResources.TerminatorBlockId)
                    break;

                Console.WriteLine(ImagingResources.SkipExtensionBlockMessage, blockSize);
                reader.BaseStream.Seek(blockSize, SeekOrigin.Current);
            }
        }
    }
}