//-----------------------------------------------------------------------
// <copyright file="DoodlDisplayModel.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.ViewModel
{
    using System.Collections.Generic;
    using Model;

    /// <summary>
    /// Provides data to render the doodl display view.
    /// </summary>
    public class DoodlDisplayModel
    {
        /// <summary>
        /// Gets or sets the doodl being displayed.
        /// </summary>
        public Doodl Doodl { get; set; }

        /// <summary>
        /// Gets or sets the doodl on which this doodl was based, if any.
        /// </summary>
        public Doodl Original { get; set; }

        /// <summary>
        /// Gets or sets a list of edits based on this doodl.
        /// </summary>
        public IList<Doodl> Edits { get; set; }
    }
}
