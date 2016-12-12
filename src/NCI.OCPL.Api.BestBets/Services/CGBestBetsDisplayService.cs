using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using NCI.OCPL.Api.BestBets;
using Microsoft.Extensions.Logging;

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
        /// 
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IBestBetDisplay GetBestBetForDisplay(string categoryID)
        {
            string requestUrl = _options.Host;
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
                    using (XmlReader xmlReader = XmlReader.Create(message.Content.ReadAsStreamAsync().Result))
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

    }
}