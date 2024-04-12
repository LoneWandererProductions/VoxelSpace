/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/Dispersion.cs
 * PURPOSE:     Helper class that handles a bit of Statistic, in this case Measures of dispersion
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:    https://welt-der-bwl.de/Streuungsma%C3%9Fe
 */

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
            if (row == null || row.Count == 0)
            {
                return;
            }

            Row = row;
            CalcArithmeticMean();
            CalcVariance();
            CalcStandardDeviation();
            CalcCoefficientOfVariation();
            CalcSpan();
            CalcMeanAbsoluteDeviation();
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
        ///     Calculates the arithmetic mean.
        /// </summary>
        private void CalcArithmeticMean()
        {
            var mean = Row.Aggregate<double, double>(0, (current, element) => element + current);

            ArithmeticMean = mean / Row.Count;
        }

        /// <summary>
        ///     Calculates the variance.
        /// </summary>
        private void CalcVariance()
        {
            var variance = Row.Select(element => Math.Pow(element - ArithmeticMean, 2))
                .Aggregate<double, double>(0, (current, cache) => cache + current);

            Variance = variance / Row.Count;
        }

        /// <summary>
        ///     Calculates the standard deviation.
        /// </summary>
        private void CalcStandardDeviation()
        {
            StandardDeviation = Math.Sqrt(Variance);
        }

        /// <summary>
        ///     Calculates the coefficient of variation.
        /// </summary>
        private void CalcCoefficientOfVariation()
        {
            CoefficientOfVariation = StandardDeviation / ArithmeticMean;
        }

        /// <summary>
        ///     Calculates the span.
        /// </summary>
        private void CalcSpan()
        {
            double max = Row[0], min = Row[0];

            foreach (var element in Row)
            {
                if (min >= element)
                {
                    min = element;
                }

                if (max <= element)
                {
                    max = element;
                }
            }

            Span = max - min;
        }

        /// <summary>
        ///     Calculates the mean absolute deviation.
        /// </summary>
        private void CalcMeanAbsoluteDeviation()
        {
            var variance = Row.Select(element => Math.Abs(element - ArithmeticMean))
                .Aggregate<double, double>(0, (current, cache) => cache + current);

            MeanAbsoluteDeviation = variance / Row.Count;
        }
    }
}
