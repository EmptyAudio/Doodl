//-----------------------------------------------------------------------
// <copyright file="IDoodlStorage.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for storing doodls.
    /// </summary>
    public interface IDoodlStorage
    {
        /// <summary>
        /// Uploads a doodl asynchronously.
        /// </summary>
        /// <param name="creator">The creator of the doodl.</param>
        /// <param name="displayImage">The display image stream of the doodl.</param>
        /// <param name="thumbnail">The thumbnail stream of the doodl.</param>
        /// <param name="ink">The source ink stream of the doodl.</param>
        /// <param name="backgroundImage">The optional background image behind the ink of the doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The uploaded doodl's display location.</returns>
        Task<string> UploadDoodlAsync(
            string creator,
            Stream displayImage,
            Stream thumbnail,
            Stream ink,
            Stream backgroundImage,
            CancellationToken cancellationToken);

        /// <summary>
        /// Uploads an edit of a doodl asynchronously.
        /// </summary>
        /// <param name="original">The identifier of the original doodl.</param>
        /// <param name="creator">The creator of the edit.</param>
        /// <param name="displayImage">The display image stream of the doodl.</param>
        /// <param name="thumbnail">The thumbnail stream of the doodl.</param>
        /// <param name="ink">The source ink stream of the doodl.</param>
        /// <param name="backgroundImage">The optional background image behind the ink of the doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The uploaded doodl's display location.</returns>
        Task<string> UploadDoodlEditAsync(
            Doodl original,
            string creator,
            Stream displayImage,
            Stream thumbnail,
            Stream ink,
            Stream backgroundImage,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a doodl asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the doodl to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The retrieved doodl.</returns>
        Task<Doodl> GetDoodlAsync(Guid id, CancellationToken cancellationToken);
    }
}
