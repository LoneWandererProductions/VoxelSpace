using System.Collections.Generic;
using System.Drawing;

namespace Imaging
{
    public class Lif
    {
        public Lif()
        {
        }

        public Lif(List<Cif> layers, List<LayerSettings> layerSettings)
        {
            Layers = layers;
            LayerSettings = layerSettings;
        }

        public List<Cif> Layers { get; set; } = new();
        public List<LayerSettings> LayerSettings { get; set; } = new();

        // Merge all visible layers to create the final image
        public Bitmap MergeLayers()
        {
            if (Layers.Count == 0)
            {
                return null;
            }

            // Start with base layer (always visible)
            var finalImage = Layers[0];

            for (var i = 1; i < Layers.Count; i++)
            {
                var settings = LayerSettings[i];
                if (settings.IsVisible)
                {
                    var deltaLayer = Layers[i];
                    //ApplyDelta(finalImage, deltaLayer, settings);
                }
            }

            return finalImage.GetImage();
        }

        // Apply delta logic for combining Cif layers
        private void ApplyDelta(Cif baseImage, Cif deltaLayer, LayerSettings settings)
        {
            foreach (var (color, pixels) in deltaLayer.CifImage)
            foreach (var pixel in pixels)
            {
                var x = pixel % baseImage.Width;
                var y = pixel / baseImage.Width;

                //var baseColor = baseImage.GetPixel(x, y);
                //var blendedColor = ApplyAlpha(baseColor, color, settings.Alpha);

                //baseImage.SetPixel(x, y, blendedColor);
            }
        }

        private Color ApplyAlpha(Color baseColor, Color deltaColor, float alpha)
        {
            var r = (int)((deltaColor.R * alpha) + (baseColor.R * (1 - alpha)));
            var g = (int)((deltaColor.G * alpha) + (baseColor.G * (1 - alpha)));
            var b = (int)((deltaColor.B * alpha) + (baseColor.B * (1 - alpha)));
            return Color.FromArgb(r, g, b);
        }
    }
}
