using System;
using System.Collections.Generic;
using System.Net.Http;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;


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

        /// <summary>
        /// Retrieves content metadata files for a named root.
        /// </summary>
        /// <param name="root">The specific root to retrieve for.</param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>
        /// Valid root values are reflected as the root parameter in the Url portion of
        /// the structure returned by ListAvailablePaths().
        /// </remarks>
        public IPublishedContentListing GetItemsForPath(string root, string path)
        {
            UriBuilder requestUri = new UriBuilder(_options.Host);
            requestUri.Path = _options.ListRoot;
            requestUri.Query = string.Format(_options.SpecificListPathFormatter, root, path);



            // Perform the actual request
            HttpResponseMessage message = _client.GetAsync(requestUri.Uri).Result;

            //Only process the message if it was successful
            if (message.IsSuccessStatusCode)
            {
                PublishedContentListing pclData = null;

                try
                {
                    // Get the content from the response message and return the deserialized object.
                    string jsonData = message.Content.ReadAsStringAsync().Result;
                    pclData = JsonConvert.DeserializeObject<PublishedContentListing>(jsonData);
                }
                catch (Exception)
                {
                    throw new APIErrorException(500, "Bad Data Structure.");
                }

                return pclData;
            }
            else
            {
                throw new APIErrorException(500, "Error connecting to search servers");
            }
        }

        /// <summary>
        /// Retrieves a list of published content paths which are available for retrieval.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPathListInfo> ListAvailablePaths()
        {
            UriBuilder requestUri = new UriBuilder(_options.Host);
            requestUri.Path = _options.ListRoot;

            // Perform the actual request
            HttpResponseMessage message = _client.GetAsync(requestUri.Uri).Result;

            //Only process the message if it was successful
            if (message.IsSuccessStatusCode)
            {
                IPathListInfo[] pathList = null;

                try
                {
                    // Get the content from the response message and return the deserialized object.
                    string jsonData = message.Content.ReadAsStringAsync().Result;
                    pathList = JsonConvert.DeserializeObject<PathListInfo[]>(jsonData);
                }
                catch (Exception)
                {
                    throw new APIErrorException(500, "Bad Data Structure.");
                }

                return pathList;
            }
            else
            {
                throw new APIErrorException(500, "Error connecting to search servers");
            }
        }
    }
}
