using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    public interface IPublishedContentListingService
    {

        IPublishedFile GetPublishedFile(Type model, string path);

        IEnumerable<IPathListInfo> ListAvailablePaths();


        IPublishedContentListing GetItemsForPath(string root, string path);
    }
}
