using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    /// <summary>
    /// Metadata for a CancerGov published content item.
    /// </summary>
    public interface IPublishedContentInfo
    {
        /// <summary>
        /// Full path to the file, relative to server root.
        /// </summary>
        string FullWebPath { get; set; }

        /// <summary>
        /// The name of the file with no path information
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Date the file was created.
        /// </summary>
        DateTime CreationTime { get; set; }

        /// <summary>
        /// Date the file was last updated.
        /// </summary>
        DateTime LastWriteTime { get; set; }
    }
}
