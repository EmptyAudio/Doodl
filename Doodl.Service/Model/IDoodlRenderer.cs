//-----------------------------------------------------------------------
// <copyright file="IDoodlRenderer.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.Model
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods for rendering a doodl's web pages.
    /// </summary>
    public interface IDoodlRenderer
    {
        /// <summary>
        /// Renders a doodl's display page.
        /// </summary>
        /// <param name="doodl">The doodl to render.</param>
        /// <param name="original">The doodl this was based on.</param>
        /// <param name="edits">The edits based on this doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The rendered page.</returns>
        Task<string> RenderDoodlDisplayAsync(Doodl doodl, Doodl original, IEnumerable<Doodl> edits, CancellationToken cancellationToken);
    }
}
