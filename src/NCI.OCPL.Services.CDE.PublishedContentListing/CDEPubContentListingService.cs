using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    public class CDEPubContentListingService : IPublishedContentListingService
    {
        private HttpClient _client;
        private PublishedContentListingServiceOptions _options;
        private readonly ILogger<CDEPubContentListingService> _logger;

        public CDEPubContentListingService(HttpClient client,
            IOptions<PublishedContentListingServiceOptions> options,
            ILogger<CDEPubContentListingService> logger)
        {
            _client = client;
            _options = options.Value;
            _logger = logger;
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
                catch (Exception ex)
                {
                    _logger.LogError("Bad Data Structure.\n{0}", ex.Message);
                    throw new APIErrorException(500, "Bad Data Structure.");
                }

                return pclData;
            }
            else
            {
                _logger.LogError("Error connecting to search servers.\nStatus: {0}, '{1}'\n{2}", message.StatusCode, message.ReasonPhrase, requestUri.Uri.ToString());
                throw new APIErrorException(500, "Error connecting to search servers.");
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
                catch (Exception ex)
                {
                    _logger.LogError("Bad Data Structure.\n{0}", ex.Message);
                    throw new APIErrorException(500, "Bad Data Structure.");
                }

                return pathList;
            }
            else
            {
                _logger.LogError("Error connecting to search servers.\nStatus: {0}, '{1}'\n{2}", message.StatusCode, message.ReasonPhrase, requestUri.Uri.ToString());
                throw new APIErrorException(500, "Error connecting to search servers");
            }
        }

        public TModel GetPublishedFile<TModel>(string path) where TModel : class
        {
            return GetPublishedFileAsync<TModel>(path).Result;
        }

        async public Task<TModel> GetPublishedFileAsync<TModel>(string path) where TModel : class
        {
            UriBuilder requestUri = new UriBuilder(_options.Host);
            requestUri.Path = path;

            HttpResponseMessage message = await _client.GetAsync(requestUri.Uri);

            //Only process the message if it was successful
            if (message.IsSuccessStatusCode)
            {
                try
                {
                    // Create the serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(TModel), "cde");

                    //Get the content from the response message and return the deserialized object.
                    using (XmlReader xmlReader = XmlReader.Create(message.Content.ReadAsStreamAsync().Result))
                    {
                        return (TModel)serializer.Deserialize(xmlReader);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Bad Data Structure.\n{0}", ex.Message);
                    throw new APIErrorException(500, "Bad XML Structure.");
                }
            }
            else
            {
                // No need to log invalid requests
                if (message.StatusCode != HttpStatusCode.NotFound)
                    _logger.LogError("Could not retrieve {0},\n'{1}' {2}", requestUri.Uri, message.StatusCode, message.ReasonPhrase);
                throw new APIErrorException(500, string.Format("Could not retrieve {0}", requestUri.Uri));
            }

        }

    }
}
