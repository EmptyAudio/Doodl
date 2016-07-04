//-----------------------------------------------------------------------
// <copyright file="Doodl.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.Model
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents an uploaded doodl.
    /// </summary>
    public class Doodl : TableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the doodl.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the creator of this doodl.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets the doodl this doodl is an edit of.
        /// </summary>
        public Guid? Original { get; set; }

        /// <summary>
        /// Gets or sets the location of the doodl display image.
        /// </summary>
        public string DisplayImageLocation { get; set; }

        /// <summary>
        /// Gets or sets the location of the thumbnail image.
        /// </summary>
        public string ThumbnailLocation { get; set; }

        /// <summary>
        /// Gets or sets the location of the doodl's source ink.
        /// </summary>
        public string InkLocation { get; set; }

        /// <summary>
        /// Gets or sets the location of the doodl's background image.
        /// </summary>
        public string BackgroundLocation { get; set; }

        /// <summary>
        /// Gets or sets the location of the doodl's display page.
        /// </summary>
        public string Location { get; set; }
    }
}