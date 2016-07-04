//-----------------------------------------------------------------------
// <copyright file="IUploadService.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for uploading doodls.
    /// </summary>
    public interface IUploadService
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
    }
}
