using System;

namespace NCI.OCPL.Api.BestBets
{
    /// <summary>
    /// A collection of metadata structures for CancerGov published content.
    /// </summary>

    public class PublishedContentListing
    {
        public string[] Directories {get; set;}

        public PublishedContentInfo[] Files {get; set;} 
    }
}