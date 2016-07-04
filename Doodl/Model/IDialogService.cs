//-----------------------------------------------------------------------
// <copyright file="IDialogService.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for showing open/close dialogs.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a file open dialog and returns a stream to the file to open if successful.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for the opened file, or <c>null</c> if the dialog was cancelled.</returns>
        Stream OpenFile();

        /// <summary>
        /// Shows a file save dialog and returns a stream to the file to save to if successful.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for the file to save to, or <c>null</c> if the dialog was cancelled.</returns>
        Stream SaveFile();

        /// <summary>
        /// Shows a file save dialog and returns a stream to the file to save an image to if successful.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for the file to save an image to, or <c>null</c> if the dialog was cancelled.</returns>
        Stream SaveFileAsImage();
    }
}
