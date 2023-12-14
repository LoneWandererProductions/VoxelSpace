/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageGif.cs
 * PURPOSE:     Extends the Image Control and adds Gif Support via SourceGif Property
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     Extension for Image to play e.g. gif Images
    /// </summary>
    /// <seealso cref="Image" />
    public sealed class ImageGif : Image
    {
        /// <summary>
        ///     The frame index property
        /// </summary>
        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register(nameof(FrameIndex), typeof(int), typeof(ImageGif),
                new UIPropertyMetadata(0, ChangingFrameIndex));

        /// <summary>
        ///     The automatic start property
        /// </summary>
        public static readonly DependencyProperty AutoStartProperty =
            DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(ImageGif),
                new UIPropertyMetadata(false, AutoStartPropertyChanged));

        /// <summary>
        ///     The GIF source property
        /// </summary>
        public static readonly DependencyProperty GifSourceProperty =
            DependencyProperty.Register(nameof(GifSource), typeof(string), typeof(ImageGif),
                new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));

        /// <summary>
        ///     The animation
        /// </summary>
        private Int32Animation _animation;

        /// <summary>
        ///     The image list
        /// </summary>
        private List<ImageSource> _imageList;

        /// <summary>
        ///     The is initialized
        /// </summary>
        private bool _isInitialized;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes the <see cref="ImageGif" /> class.
        /// </summary>
        static ImageGif()
        {
            VisibilityProperty.OverrideMetadata(typeof(ImageGif),
                new FrameworkPropertyMetadata(VisibilityPropertyChanged));
        }

        /// <summary>
        ///     Gets or sets the index of the frame.
        /// </summary>
        /// <value>
        ///     The index of the frame.
        /// </value>
        public int FrameIndex
        {
            get => (int)GetValue(FrameIndexProperty);
            set => SetValue(FrameIndexProperty, value);
        }

        /// <summary>
        ///     Defines whether the animation starts on it's own
        /// </summary>
        public bool AutoStart
        {
            get => (bool)GetValue(AutoStartProperty);
            set => SetValue(AutoStartProperty, value);
        }

        /// <summary>
        ///     Gets or sets the GIF source.
        /// </summary>
        /// <value>
        ///     The GIF source.
        /// </value>
        public string GifSource
        {
            get => (string)GetValue(GifSourceProperty);
            set => SetValue(GifSourceProperty, value);
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            //check if Image exists
            if (!File.Exists(GifSource))
            {
                return;
            }

            var info = ImageGifHandler.GetImageInfo(GifSource);

            //Todo Error News perhaps
            if (info == null)
            {
                return;
            }

            _imageList = ImageGifHandler.LoadGif(GifSource);

            Source = _imageList[0];

            if (!info.IsAnimated)
            {
                return;
            }

            var time = info.Frames / 10;

            if (time < 1)
            {
                time = 1;
            }

            _animation = new Int32Animation(0, info.Frames - 1,
                new Duration(new TimeSpan(0, 0, 0, time, 0))) { RepeatBehavior = RepeatBehavior.Forever };

            Source = _imageList[0];

            _isInitialized = true;

            if (AutoStart)
            {
                StartAnimation();
            }
        }

        /// <summary>
        ///     Visibilities the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                ((ImageGif)sender).StartAnimation();
            }
            else
            {
                ((ImageGif)sender).StopAnimation();
            }
        }

        /// <summary>
        ///     Changing the index of the frame.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="ev">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            if (!((ImageGif)obj).AutoStart)
            {
                return;
            }

            if (obj is ImageGif gifImage)
            {
                gifImage.Source = gifImage._imageList[(int)ev.NewValue];
            }
        }

        /// <summary>
        ///     Automatics the start property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ImageGif)?.StartAnimation();
        }

        /// <summary>
        ///     GIFs the source property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ImageGif)?.Initialize();
        }

        /// <summary>
        ///     Starts the animation
        /// </summary>
        private void StartAnimation()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            BeginAnimation(FrameIndexProperty, _animation);
        }

        /// <summary>
        ///     Stops the animation
        /// </summary>
        public void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }
    }
}
