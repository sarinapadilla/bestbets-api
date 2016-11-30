using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    interface IPublishedContentListingService
    {
        // TODO return type
        IEnumerable<IPathListInfo> ListAvailablePaths();

        // TODO return type
        IPublishedContentListing GetItemsForPath(string root, string path);
    }
}
