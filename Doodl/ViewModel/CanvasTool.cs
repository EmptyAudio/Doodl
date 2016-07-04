//-----------------------------------------------------------------------
// <copyright file="CanvasTool.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.ViewModel
{
    /// <summary>
    /// Tools available for editing a doodle.
    /// </summary>
    public enum CanvasTool
    {
        /// <summary>
        /// Draw new strokes.
        /// </summary>
        Ink,

        /// <summary>
        /// Draws new highlighter strokes.
        /// </summary>
        Highlighter,

        /// <summary>
        /// Erase parts of strokes.
        /// </summary>
        Eraser,

        /// <summary>
        /// Erase whole strokes.
        /// </summary>
        StrokeEraser,
    }
}
