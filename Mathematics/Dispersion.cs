/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/Dispersion.cs
 * PURPOSE:     Helper class that handles a bit of Statistic, in this case Measures of dispersion
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:    https://welt-der-bwl.de/Streuungsma%C3%9Fe
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace Mathematics
{
    /// <summary>
    ///     Just some basic Statistics functions
    /// </summary>
    public sealed class Dispersion
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Dispersion" /> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public Dispersion(List<double> row)
        {
            if (row == null || row.Count == 0) throw new ArgumentException(MathResources.StatisticsErrorInput);

            Row = row;
            CalculateStatistics();
        }

        /// <summary>
        ///     Gets the row.
        /// </summary>
        /// <value>
        ///     The row.
        /// </value>
        private List<double> Row { get; }

        /// <summary>
        ///     Gets the arithmetic mean.
        /// </summary>
        /// <value>
        ///     The arithmetic mean.
        /// </value>
        public double ArithmeticMean { get; private set; }

        /// <summary>
        ///     Gets the variance.
        /// </summary>
        /// <value>
        ///     The variance.
        /// </value>
        public double Variance { get; private set; }

        /// <summary>
        ///     Gets the standard deviation.
        /// </summary>
        /// <value>
        ///     The standard deviation.
        /// </value>
        public double StandardDeviation { get; private set; }

        /// <summary>
        ///     Gets or sets the coefficient of variation.
        /// </summary>
        /// <value>
        ///     The coefficient of variation.
        /// </value>
        public double CoefficientOfVariation { get; private set; }

        /// <summary>
        ///     Gets or sets the span.
        /// </summary>
        /// <value>
        ///     The span.
        /// </value>
        public double Span { get; private set; }

        /// <summary>
        ///     Gets or sets the mean absolute deviation.
        /// </summary>
        /// <value>
        ///     The mean absolute deviation.
        /// </value>
        public double MeanAbsoluteDeviation { get; private set; }

        /// <summary>
        ///     Gets the median.
        /// </summary>
        /// <value>
        ///     The median.
        /// </value>
        public double Median { get; private set; }

        /// <summary>
        ///     Gets the mode.
        /// </summary>
        /// <value>
        ///     The mode.
        /// </value>
        public double Mode { get; private set; }

        /// <summary>
        ///     Calculates the statistics.
        /// </summary>
        private void CalculateStatistics()
        {
            CalculateArithmeticMean();
            CalculateVariance();
            CalculateStandardDeviation();
            CalculateCoefficientOfVariation();
            CalculateSpan();
            CalculateMeanAbsoluteDeviation();
            CalculateMedian();
            CalculateMode();
        }

        /// <summary>
        ///     Calculates the arithmetic mean.
        /// </summary>
        private void CalculateArithmeticMean()
        {
            ArithmeticMean = Row.Average();
        }

        /// <summary>
        ///     Calculates the variance.
        /// </summary>
        private void CalculateVariance()
        {
            Variance = Row.Average(element => Math.Pow(element - ArithmeticMean, 2));
        }

        /// <summary>
        ///     Calculates the standard deviation.
        /// </summary>
        private void CalculateStandardDeviation()
        {
            StandardDeviation = Math.Sqrt(Variance);
        }

        /// <summary>
        ///     Calculates the coefficient of variation.
        /// </summary>
        private void CalculateCoefficientOfVariation()
        {
            CoefficientOfVariation = StandardDeviation / ArithmeticMean;
        }

        /// <summary>
        ///     Calculates the span.
        /// </summary>
        private void CalculateSpan()
        {
            Span = Row.Max() - Row.Min();
        }

        /// <summary>
        ///     Calculates the mean absolute deviation.
        /// </summary>
        private void CalculateMeanAbsoluteDeviation()
        {
            MeanAbsoluteDeviation = Row.Average(element => Math.Abs(element - ArithmeticMean));
        }

        /// <summary>
        ///     Calculates the median.
        /// </summary>
        private void CalculateMedian()
        {
            var sortedList = Row.OrderBy(x => x).ToList();
            var n = sortedList.Count;
            Median = n % 2 == 0 ? (sortedList[n / 2 - 1] + sortedList[n / 2]) / 2.0 : sortedList[n / 2];
        }

        /// <summary>
        ///     Calculates the mode.
        /// </summary>
        private void CalculateMode()
        {
            var groups = Row.GroupBy(x => x);
            var maxCount = groups.Max(g => g.Count());
            Mode = groups.First(g => g.Count() == maxCount).Key;
        }
    }
}