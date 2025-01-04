namespace Imaging
{
    public class LayerSettings
    {
        public LayerSettings()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LayerSettings" /> class.
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="alpha">The alpha.</param>
        public LayerSettings(bool isVisible, float alpha)
        {
            IsVisible = isVisible;
            Alpha = alpha;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; set; } = true; // Determines if the layer is visible

        /// <summary>
        ///     Gets or sets the alpha.
        /// </summary>
        /// <value>
        ///     The alpha.
        /// </value>
        public float Alpha { get; set; } = 1.0f; // Transparency level (0 = fully transparent, 1 = fully opaque)

        /// <summary>
        ///     Gets the name of the layer.
        /// </summary>
        /// <value>
        ///     The name of the layer.
        /// </value>
        public string LayerName { get; internal set; }
    }
}
