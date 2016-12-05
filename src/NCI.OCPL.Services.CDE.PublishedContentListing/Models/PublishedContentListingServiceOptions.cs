using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    public class PublishedContentListingServiceOptions
    {
        /// <summary>
        /// This is the host, with protocol, of where to fetch info from.
        /// </summary>
        /// <returns></returns>
        public string Host { get; set; }

        /// <summary>
        /// This is the root path for retrieving lists.
        /// </summary>
        public string ListRoot { get; set; }

        /// <summary>
        /// This is the path to use when retrieving the list of files
        /// for a specific list and path
        /// </summary>
        public string SpecificListPathFormatter { get; set; }
    }
}
