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
                } catch (Exception ex) {
                    //TODO: Replace this with better exception.
                    throw new Exception("Bad XML");
                }
            } 
            else 
            {
                //TODO: Replace this with better exception.
                throw new Exception(string.Format("Could not retrieve {0}", requestUrl));
            }
        }

        public IEnumerable<IBestBetCategory> GetAllBestBetsForIndexing()
        {
            string requestUrl = _options.Host;
            requestUrl += _options.BBCategoryPathFormatter;

            HttpResponseMessage message = _client.GetAsync(requestUrl).Result;

            //Only process the message if it was successful
            if (message.IsSuccessStatusCode) 
            {
                try {


                    // Create the serializer
                    //XmlSerializer serializer = new XmlSerializer(typeof(CancerGovBestBet), "cde");

                    //Get the content from the response message and return the deserialized object.
                    using (TextReader reader = new StreamReader(message.Content.ReadAsStreamAsync().Result)) 
                    {

                        PublishedContentListing list = JsonConvert.DeserializeObject<PublishedContentListing>(reader.ToString());
                        
//                        return (IBestBetDisplay)serializer.Deserialize(xmlReader);
                            return null;
                    }
                } catch (Exception ex) {
                    //TODO: Replace this with better exception.
                    throw new Exception("Bad XML");
                }
            } 
            else 
            {
                //TODO: Replace this with better exception.
                throw new Exception(string.Format("Could not retrieve {0}", requestUrl));
            }
            return null;           

            //throw new NotImplementedException();
        }
        
    }
}