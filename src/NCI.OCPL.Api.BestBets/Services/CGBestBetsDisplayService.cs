using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using NCI.OCPL.Api.BestBets;

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

        /// <summary>
        /// Creates a new instance of a CancerGovBestBetsClient
        /// </summary>
        /// <param name="client">The client to be used for connections</param>
        public CGBestBetsDisplayService(HttpClient client, IOptions<CGBestBetsDisplayServiceOptions> options) {
            _client = client;
            _options = options.Value;
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
                try {
                    // Create the serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(CancerGovBestBet), "cde");

                    //Get the content from the response message and return the deserialized object.
                    using (XmlReader xmlReader = XmlReader.Create(message.Content.ReadAsStreamAsync().Result)) 
                    {
                        return (IBestBetDisplay)serializer.Deserialize(xmlReader);
                    }
                } catch (Exception) {
                    throw new APIErrorException(500, "Bad XML Structure.");
                }
            } 
            else 
            {
               throw new APIErrorException(500, string.Format("Could not retrieve {0}", requestUrl));
            }
        }

        /// <summary>
        /// Retrieves metadata for all existing best bets for indexing.
        /// </summary>
        public IEnumerable<PublishedContentInfo> GetAllBestBetsForIndexing()
        {
            string requestUrl = _options.Host;
            requestUrl += _options.BBCategoryPathFormatter;

            HttpResponseMessage message = _client.GetAsync(requestUrl).Result;

            //Only process the message if it was successful
            if (message.IsSuccessStatusCode) 
            {
                PublishedContentListing list = null;

                try 
                {
                    // Get the content from the response message and return the deserialized object.
                    string jsonData = message.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<PublishedContentListing>(jsonData);
                }
                catch (Exception)
                {
                    throw new APIErrorException(500, "Bad Data Structure.");
                }

                foreach(PublishedContentInfo file in list.Files)
                {
                    yield return file;
                }
            } 
            else 
            {
                throw new APIErrorException(500, "Error connecting to search servers");
            }
        }
        
    }
}