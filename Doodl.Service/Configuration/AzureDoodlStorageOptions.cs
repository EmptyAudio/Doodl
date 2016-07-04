//-----------------------------------------------------------------------
// <copyright file="AzureDoodlStorageOptions.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.Configuration
{
    using System;

    /// <summary>
    /// Provides configuration options for storing Doodls in Windows Azure Storage.
    /// </summary>
    public class AzureDoodlStorageOptions
    {
        /// <summary>
        /// Gets or sets the storage connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the doodl container format string.
        /// </summary>
        public string ContainerFormat { get; set; }

        /// <summary>
        /// Gets or sets the doodl display image blob name format string.
        /// </summary>
        public string DisplayImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail blob name format string.
        /// </summary>
        public string ThumbnailFormat { get; set; }

        /// <summary>
        /// Gets or sets the ink blob name format string.
        /// </summary>
        public string InkFormat { get; set; }

        /// <summary>
        /// Gets or sets the background image blob name format string.
        /// </summary>
        public string BackgroundFormat { get; set; }

        /// <summary>
        /// Gets or sets the doodl display page blob name format string.
        /// </summary>
        public string DisplayPageFormat { get; set; }

        /// <summary>
        /// Gets or sets the name of the table to use for storing doodl information.
        /// </summary>
        public string DoodlTableName { get; set; }

        /// <summary>
        /// Gets or sets the name of the table to use for storing doodl edits.
        /// </summary>
        public string DoodlEditsTableName { get; set; }
    }
}