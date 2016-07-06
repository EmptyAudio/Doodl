//-----------------------------------------------------------------------
// <copyright file="IDoodlService.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for interacting with the doodl service.
    /// </summary>
    public interface IDoodlService
    {
        /// <summary>
        /// Uploads a doodl.
        /// </summary>
        /// <param name="doodlerName">The name to put on the doodl.</param>
        /// <param name="imageStream">The image stream to upload.</param>
        /// <param name="thumbnailStream">The thumbnail stream to upload.</param>
        /// <param name="inkStream">The ink stream to upload.</param>
        /// <returns>The URL of the uploaded doodl.</returns>
        Task<string> Upload(string doodlerName, Stream imageStream, Stream thumbnailStream, Stream inkStream);

        /// <summary>
        /// Uploads an edit of a doodl.
        /// </summary>
        /// <param name="original">The ID of the original doodl.</param>
        /// <param name="doodlerName">The name to put on the doodl.</param>
        /// <param name="imageStream">The image stream to upload.</param>
        /// <param name="thumbnailStream">The thumbnail stream to upload.</param>
        /// <param name="inkStream">The ink stream to upload.</param>
        /// <returns>The URL of the uploaded doodl.</returns>
        Task<string> UploadEdit(Guid original, string doodlerName, Stream imageStream, Stream thumbnailStream, Stream inkStream);

        /// <summary>
        /// Retrieves the source strokes for a given uploaded doodl.
        /// </summary>
        /// <param name="id">The ID of the doodl to retrieve.</param>
        /// <returns>A stream of ink data for the given uploaded doodl.</returns>
        Task<Stream> GetStrokesFor(Guid id);
    }
}
