//using ComputeSharp;

//public static class ImageProcessor
//{
//    public static WriteableBitmap ProcessWithComputeSharp(Bitmap image)
//    {
//        int width = image.Width;
//        int height = image.Height;
//        var pixels = new int[width * height];

//        // Load image into pixels array
//        BitmapData data = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
//        System.Runtime.InteropServices.Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
//        image.UnlockBits(data);

//        // Use ComputeSharp to process pixels
//        using (var buffer = GraphicsDevice.Default.AllocateReadWriteTexture2D<int, float4>(width, height))
//        {
//            buffer.CopyFrom(pixels);

//            // Apply a GPU shader to make the image grayscale
//            GraphicsDevice.Default.ForEach(buffer, static (ref float4 pixel) =>
//            {
//                float gray = 0.3f * pixel.X + 0.59f * pixel.Y + 0.11f * pixel.Z;
//                pixel = new float4(gray, gray, gray, pixel.W);
//            });

//            // Copy processed pixels back
//            buffer.CopyTo(pixels);
//        }

//        // Convert back to Bitmap
//        var processedBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
//        processedBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

//        return processedBitmap;
//    }
//}



