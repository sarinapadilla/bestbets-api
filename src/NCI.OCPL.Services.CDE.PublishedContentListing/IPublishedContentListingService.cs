using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    public interface IPublishedContentListingService
    {

        TModel GetPublishedFile<TModel>(string path) where TModel : class;

        IEnumerable<IPathListInfo> ListAvailablePaths();


        IPublishedContentListing GetItemsForPath(string root, string path);
    }
}
