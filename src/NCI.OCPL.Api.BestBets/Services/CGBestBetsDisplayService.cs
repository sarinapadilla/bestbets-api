using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using NCI.OCPL.Api.BestBets;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Services
{
    /// <summary>
    /// This class defines a client that can be used to fetch best bets data from CGov.
    /// </summary>
    /// <remarks>
    /// This client is designed to be reused, so don't create a new one for each connection
    /// to cgov.  (And don't make any code that would actually break this...)
    /// </remarks>
    public class CGBestBetsDisplayService : IBestBetsDisplayService
    {
        private HttpClient _client;
        private CGBestBetsDisplayServiceOptions _options;
        private readonly ILogger<CGBestBetsDisplayService> _logger;

        /// <summary>
        /// Creates a new instance of a CancerGovBestBetsClient
        /// </summary>
        /// <param name="client">The client to be used for connections</param>
        public CGBestBetsDisplayService(HttpClient client,
            IOptions<CGBestBetsDisplayServiceOptions> options,
            ILogger<CGBestBetsDisplayService> logger) {
            _client = client;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Gets the best bets category list asynchronously
        /// </summary>
        /// <param name="collection">The collection to use. This will be 'live' or 'preview'.</param>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public async Task<IBestBetDisplay> GetBestBetForDisplay(string collection, string categoryID)
        {            
            string requestUrl = (collection == "preview") ?
                _options.PreviewHost :
                _options.LiveHost;
            
            requestUrl += _options.BBCategoryPathFormatter;
            requestUrl = string.Format(requestUrl, categoryID);

            HttpResponseMessage message = _client.GetAsync(requestUrl).Result;

            //Only process the message if it was successful
            if (message.IsSuccessStatusCode)
            {
                try
                {
                    // Create the serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(CancerGovBestBet), "cde");

                    //Get the content from the response message and return the deserialized object.
                    using (XmlReader xmlReader = XmlReader.Create(await message.Content.ReadAsStreamAsync()))
                    {
                        return (IBestBetDisplay)serializer.Deserialize(xmlReader);
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
                _logger.LogError("Error connecting to search servers.\nStatus: {0}, '{1}'\n{2}", message.StatusCode, message.ReasonPhrase, requestUrl);
                throw new APIErrorException(500, string.Format("Could not retrieve {0}", requestUrl));
            }
        }

        /// <summary>
        /// True if CGBestBetsDisplayService is able to retrieve BestBets.
        /// </summary>
        public async Task<bool> IsHealthy()
        {

            var hostChecks = new Task<bool>[]
            {
                this.IsHostHealthy(_options.LiveHost),
                this.IsHostHealthy(_options.PreviewHost),
            };

            return (await Task.WhenAll(hostChecks))
                    .Aggregate(
                        true, 
                        (res, next) => res && next 
                    );            
        }

        /// <summary>
        /// Checks if a single host is healthy
        /// </summary>
        /// <returns>The host healthy.</returns>
        private async Task<bool> IsHostHealthy(string host) {
            
            UriBuilder requestUrl = new UriBuilder(host);
            requestUrl.Path = _options.HealthCheckPath;

            try
            {
                HttpResponseMessage message = await _client.GetAsync(requestUrl.Uri);

                if (message.IsSuccessStatusCode && message.StatusCode == HttpStatusCode.OK)
                {
                    //This is the only condition that will return true.
                    return true;
                }
                else
                {
                    _logger.LogError($"Unable to fetch URL '{requestUrl.Uri.ToString()}'.");
                    _logger.LogError($"Status {message.StatusCode}, {message.ReasonPhrase}");
                }

            } catch (Exception ex) {
                //WE need to swallow the error and just return false.
                _logger.LogError($"Unable to fetch URL '{requestUrl.Uri.ToString()}'.");
                _logger.LogError($"Error: {ex.Message}" );
            }

            return false;
        }
    }
}