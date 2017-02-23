using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Indexer.Services
{
    public class ConfiguredElasticClientFactory : IElasticClientFactory 
    {
        private readonly ElasticSearchOptions _config;

        public ConfiguredElasticClientFactory(IOptions<ElasticSearchOptions> config)
        {
            _config = config.Value;
        }

        public IElasticClient GetInstance()
        {
            // Get the ElasticSearch credentials.
            string username = _config.Userid;
            string password = _config.Password;

            //Get the ElasticSearch servers that we will be connecting to.
            List<Uri> uris = GetServerUriList();

            // Create the connection pool, the SniffingConnectionPool will 
            // keep tabs on the health of the servers in the cluster and
            // probe them to ensure they are healthy.  This is how we handle
            // redundancy and load balancing.
            var connectionPool = new SniffingConnectionPool(uris);

            //Return a new instance of an ElasticClient with our settings
            ConnectionSettings settings = new ConnectionSettings(connectionPool)
                .MaximumRetries(_config.MaximumRetries);

            //Let's only try and use credentials if the username is set.
            if (!string.IsNullOrWhiteSpace(username))
            {
                settings.BasicAuthentication(username, password);
            }

            return new ElasticClient(settings);
        }

        /// <summary>
        /// Retrieves a list of Elasticsearch server URIs from the configuration's Elasticsearch:Servers setting. 
        /// </summary>
        /// <returns>Returns a list of one or more Uri objects representing the configured set of Elasticsearch servers</returns>
        /// <remarks>
        /// The configuration's Elasticsearch:Servers property is required to contain URIs for one or more Elasticsearch servers.
        /// Each URI must include a protocol (http or https), a server name, and optionally, a port number.
        /// Multiple URIs are separated by a comma.  (e.g. "https://fred:9200, https://george:9201, https://ginny:9202")
        /// 
        /// Throws ConfigurationException if no servers are configured.
        ///
        /// Throws UriFormatException if any of the configured server URIs are not formatted correctly.
        /// </remarks>
        private List<Uri> GetServerUriList()
        {
            List<Uri> uris = new List<Uri>();

            string serverList = _config.Servers;
            if (!String.IsNullOrWhiteSpace(serverList))
            {
                // Convert the list of servers into a list of Uris.
                string[] names = serverList.Split(',');
                uris.AddRange(names.Select(server => new Uri(server)));
            }
            else
            {
                throw new Exception("No servers configured");
            }

            return uris;
        }

    }
}
