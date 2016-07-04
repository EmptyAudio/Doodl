//-----------------------------------------------------------------------
// <copyright file="DoodlController.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Model;

    /// <summary>
    /// Handles doodl upload and download requests.
    /// </summary>
    public class DoodlController : Controller
    {
        private readonly IOptions<List<string>> prinnies;
        private readonly IDoodlStorage doodlStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoodlController"/> class.
        /// </summary>
        /// <param name="doodlStorage">The doodl storage provider to use.</param>
        /// <param name="prinnies">The list of prinnies to use.</param>
        public DoodlController(IDoodlStorage doodlStorage, IOptions<List<string>> prinnies)
        {
            this.doodlStorage = doodlStorage;
            this.prinnies = prinnies;
        }

        /// <summary>
        /// Uploads a new doodl.
        /// </summary>
        /// <param name="creator">The creator of the doodl.</param>
        /// <param name="displayImage">The display image of the doodl.</param>
        /// <param name="thumbnail">The thumbnail image of the doodl.</param>
        /// <param name="ink">The source ink of the doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <param name="backgroundImage">The optional background image of the doodl.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public async Task<IActionResult> Upload(
            [FromForm] string creator,
            [FromForm] IFormFile displayImage,
            [FromForm] IFormFile thumbnail,
            [FromForm] IFormFile ink,
            CancellationToken cancellationToken,
            [FromForm] IFormFile backgroundImage = null)
        {
            if (displayImage == null || thumbnail == null || ink == null)
            {
                return this.BadRequest();
            }

            var result = await this.doodlStorage.UploadDoodlAsync(
                creator,
                displayImage.OpenReadStream(),
                thumbnail.OpenReadStream(),
                ink.OpenReadStream(),
                backgroundImage?.OpenReadStream(),
                cancellationToken);

            return this.Json(new { Location = result });
        }

        /// <summary>
        /// Uploads a new doodl edit.
        /// </summary>
        /// <param name="original">The original doodl.</param>
        /// <param name="creator">The creator of the doodl.</param>
        /// <param name="displayImage">The display image of the edited doodl.</param>
        /// <param name="thumbnail">The thumbnail image of the doodl.</param>
        /// <param name="ink">The source ink of the edited doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <param name="backgroundImage">The optional background image of the edited doodl.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public async Task<IActionResult> UploadEdit(
            [FromForm] Guid original,
            [FromForm] string creator,
            [FromForm] IFormFile displayImage,
            [FromForm] IFormFile thumbnail,
            [FromForm] IFormFile ink,
            CancellationToken cancellationToken,
            [FromForm] IFormFile backgroundImage = null)
        {
            if (displayImage == null || thumbnail == null || ink == null)
            {
                return this.BadRequest();
            }

            var originalDoodl = await this.doodlStorage.GetDoodlAsync(original, cancellationToken);

            if (originalDoodl != null)
            {
                var result = await this.doodlStorage.UploadDoodlEditAsync(
                    originalDoodl,
                    creator,
                    displayImage.OpenReadStream(),
                    thumbnail.OpenReadStream(),
                    ink.OpenReadStream(),
                    backgroundImage?.OpenReadStream(),
                    cancellationToken);

                return this.Json(new { Location = result });
            }

            return this.BadRequest();
        }

        /// <summary>
        /// Retrieves a random prinny watermark.
        /// </summary>
        /// <returns>A redirect to a random prinny.</returns>
        public IActionResult Prinny()
        {
            var rng = new Random();

            return this.Redirect(this.prinnies.Value[rng.Next(this.prinnies.Value.Count)]);
        }
    }
}