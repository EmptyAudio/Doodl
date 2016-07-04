//-----------------------------------------------------------------------
// <copyright file="PenTip.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Ink;
    using System.Windows.Media;

    /// <summary>
    /// View model for the <see cref="PenTip"/> view.
    /// </summary>
    public class PenTip : INotifyPropertyChanged
    {
        private double width = 1.0d;
        private double height = 1.0d;
        private StylusTip tip;
        private bool isPressureSensitive;
        private bool smoothing;

        /// <summary>
        /// Raised when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the width of the pen tip.
        /// </summary>
        public double Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the height of the pen tip.
        /// </summary>
        public double Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the shape of the pen tip.
        /// </summary>
        public StylusTip Tip
        {
            get
            {
                return this.tip;
            }

            set
            {
                this.tip = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pen tip is pressure sensitive.
        /// </summary>
        public bool IsPressureSensitive
        {
            get
            {
                return this.isPressureSensitive;
            }

            set
            {
                this.isPressureSensitive = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pen strokes should be smoothed.
        /// </summary>
        public bool Smoothing
        {
            get
            {
                return this.smoothing;
            }

            set
            {
                this.smoothing = value;

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
