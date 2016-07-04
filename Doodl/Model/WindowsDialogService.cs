//-----------------------------------------------------------------------
// <copyright file="WindowsDialogService.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System.IO;
    using Microsoft.Win32;

    /// <summary>
    /// Provides methods for showing open/close dialogs using <see cref="OpenFileDialog"/> and <see cref="SaveFileDialog"/>.
    /// </summary>
    public class WindowsDialogService : IDialogService
    {
        private readonly OpenFileDialog openDialog;
        private readonly SaveFileDialog saveDialog;
        private readonly SaveFileDialog saveAsImageDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsDialogService"/> class.
        /// </summary>
        public WindowsDialogService()
        {
            this.openDialog = new OpenFileDialog()
            {
                Filter = "Doodl Files (*.doodl)|*.doodl|All Files (*.*)|*.*",
                Title = "Open Doodl",
            };

            this.saveDialog = new SaveFileDialog()
            {
                Filter = "Doodl Files (*.doodl)|*.doodl|All Files (*.*)|*.*",
                Title = "Save Doodl",
            };

            this.saveAsImageDialog = new SaveFileDialog()
            {
                Filter = "PNG Image (*.png)|*.png|All Files (*.*)|*.*",
                Title = "Save Doodl as Image",
            };
        }

        /// <summary>
        /// Shows a file open dialog and returns a stream to the file to open if successful.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for the opened file, or <c>null</c> if the dialog was cancelled.</returns>
        public Stream OpenFile()
        {
            if (this.openDialog.ShowDialog().GetValueOrDefault(false))
            {
                return this.openDialog.OpenFile();
            }

            return null;
        }

        /// <summary>
        /// Shows a file save dialog and returns a stream to the file to save to if successful.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for the file to save to, or <c>null</c> if the dialog was cancelled.</returns>
        public Stream SaveFile()
        {
            if (this.saveDialog.ShowDialog().GetValueOrDefault(false))
            {
                return this.saveDialog.OpenFile();
            }

            return null;
        }

        /// <summary>
        /// Shows a file save dialog and returns a stream to the file to save an image to if successful.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for the file to save an image to, or <c>null</c> if the dialog was cancelled.</returns>
        public Stream SaveFileAsImage()
        {
            if (this.saveAsImageDialog.ShowDialog().GetValueOrDefault(false))
            {
                return this.saveAsImageDialog.OpenFile();
            }

            return null;
        }
    }
}
