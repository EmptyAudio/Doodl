//-----------------------------------------------------------------------
// <copyright file="DrawingAttributesMultiValueConverter.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Ink;
    using System.Windows.Media;
    using ViewModel;

    /// <summary>
    /// Converts tool selection to a <see cref="DrawingAttributes"/>.
    /// </summary>
    public class DrawingAttributesMultiValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a group of tool selection values to a <see cref="DrawingAttributes"/>.
        /// </summary>
        /// <param name="values">The tool selection values to convert.</param>
        /// <param name="targetType">Must be <see cref="DrawingAttributes"/>.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted <see cref="DrawingAttributes"/>.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => v == null))
            {
                return new DrawingAttributes();
            }

            var tool = (CanvasTool)values[0];
            var inkColor = (Color)values[1];
            var inkPen = values[2] as PenTip;
            var highlighterColor = (Color)values[3];
            var highlighterPen = values[4] as PenTip;

            switch (tool)
            {
                case CanvasTool.Ink:
                    return new DrawingAttributes()
                    {
                        Color = inkColor,
                        IsHighlighter = false,
                        Width = inkPen.Width,
                        Height = inkPen.Height,
                        IgnorePressure = !inkPen.IsPressureSensitive,
                        FitToCurve = inkPen.Smoothing,
                        StylusTip = inkPen.Tip,
                    };
                case CanvasTool.Highlighter:
                    return new DrawingAttributes()
                    {
                        Color = highlighterColor,
                        IsHighlighter = true,
                        Width = highlighterPen.Width,
                        Height = highlighterPen.Height,
                        IgnorePressure = !highlighterPen.IsPressureSensitive,
                        FitToCurve = highlighterPen.Smoothing,
                        StylusTip = highlighterPen.Tip,
                    };
                default:
                    return new DrawingAttributes();
            }
        }

        /// <summary>
        /// This method is not implemented.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetTypes">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>Throws a <see cref="NotImplementedException"/>.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
