using System;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Extensions.Options;

using NCI.OCPL.Services.BestBets;

namespace NCI.OCPL.Services.BestBets.Services
{
    /// <summary>
    /// This class defines a client that can be used to fetch best bets data from CGov.
    /// </summary>
    /// <remarks>
    /// This client is designed to be reused, so don't create a new one for each connection
    /// to cgov.  (And don't make any code that would actually break this...)
    /// </remarks>
    public class CancerGovBestBetsClient : IBestBetsClient
    {
        private HttpClient _client;
        private CancerGovBestBetsClientOptions _options;

        /// <summary>
        /// Creates a new instance of a CancerGovBestBetsClient
        /// </summary>
        /// <param name="client">The client to be used for connections</param>
        public CancerGovBestBetsClient(HttpClient client, IOptions<CancerGovBestBetsClientOptions> options) {
            _client = client;
            _options = options.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IBestBetDisplay GetBestBetForDisplay(string categoryID) {

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
        
    }
}