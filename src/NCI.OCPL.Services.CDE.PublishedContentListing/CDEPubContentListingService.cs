using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;


namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    public class CDEPubContentListingService : IPublishedContentListingService
    {
        private HttpClient _client;
        private PublishedContentListingServiceOptions _options;

        public CDEPubContentListingService(HttpClient client, IOptions<PublishedContentListingServiceOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public IPublishedContentListing GetItemsForPath(string root, string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPathListInfo> ListAvailablePaths()
        {
            throw new NotImplementedException();
        }
    }
}
