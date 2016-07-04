//-----------------------------------------------------------------------
// <copyright file="EnumSelectedVisibilityConverter.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using ViewModel;

    /// <summary>
    /// Converts an enum value to a boolean based on a match with the supplied parameter.
    /// </summary>
    public class EnumSelectedVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts the given enum value to a boolean if it matches the supplied parameter.
        /// </summary>
        /// <param name="value">The enum value to convert.</param>
        /// <param name="targetType">The type of the enum.</param>
        /// <param name="parameter">The enum value to match.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns><c>true</c> if the value matches the parameter; otherwise <c>false</c>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var desired = Enum.Parse(value.GetType(), parameter.ToString());

            return desired.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a boolean back to the parameter enum value if <c>true</c>.
        /// </summary>
        /// <param name="value">The boolean value to convert back.</param>
        /// <param name="targetType">The type of the enum.</param>
        /// <param name="parameter">The enum value to convert back to.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The parameter enum value if the value if the value is <c>true</c>; otherwise returns <see cref="DependencyProperty.Unset"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var desired = Enum.Parse(targetType, parameter.ToString());

            return value.ToString() == Visibility.Visible.ToString() ? desired : DependencyProperty.UnsetValue;
        }
    }
}
