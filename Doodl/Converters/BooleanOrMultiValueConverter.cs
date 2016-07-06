//-----------------------------------------------------------------------
// <copyright file="BooleanOrMultiValueConverter.cs" company="EmptyAudio">
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
    /// Combines several boolean values into a single one through inclusive disjunction (OR).
    /// </summary>
    public class BooleanOrMultiValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Combines several boolean values into a single one through inclusive disjunction (OR).
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <param name="targetType">Must be <see cref="bool"/>.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted <see cref="DrawingAttributes"/>.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(v => v is bool && (bool)v);
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
