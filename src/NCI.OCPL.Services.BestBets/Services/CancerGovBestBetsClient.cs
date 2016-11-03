
using System.Net.Http;

using NCI.OCPL.Services.Bestbets;

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

        /// <summary>
        /// Creates a new instance of a CancerGovBestBetsClient
        /// </summary>
        /// <param name="client">The client to be used for connections</param>
        public CancerGovBestBetsClient(HttpClient client) {
            _client = client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IBestBetDisplay GetBestBetForDisplay(string categoryID) {
            throw new NotImplementedException();
        }
        
    }
}